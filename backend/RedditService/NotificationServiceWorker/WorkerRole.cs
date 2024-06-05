using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Common;
using HealthMonitoringServiceWorker;
using Microsoft.Azure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

namespace NotificationServiceWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private CloudQueue _queue;
        private CloudTable notificationTable;
        private CloudTable healthCheckTable;
        private CloudStorageAccount _cloudStorageAccount;
        private CommentDataRepositorycs _commentRepository;
        private SubscriptionDataRepository _subscriptionRepository;
        private UserDataRepository _userRepository;
        private string _smtpServer;
        private int _smtpPort;
        private string _smtpUser;
        private string _smtpPassword;
        private List<string> _healthCheckUrls = new List<string>
        {
            "http://redditservice/health-monitoring",
            "http://notificationservice/health-monitoring"
        };
        private List<string> _alertEmails = new List<string> { "admin@example.com" }; // Konfigurabilne email adrese

        public override void Run()
        {
            Trace.TraceInformation("NotificationServiceWorker is running");

            // Start the health check server in a new thread
            Task.Run(() => StartHealthCheckServer());

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            bool result = base.OnStart();

            _cloudStorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            var queueClient = _cloudStorageAccount.CreateCloudQueueClient();
            _queue = queueClient.GetQueueReference("notifications");
            _queue.CreateIfNotExists();

            _commentRepository = new CommentDataRepositorycs();
            _subscriptionRepository = new SubscriptionDataRepository();
            _userRepository = new UserDataRepository();

            var tableClient = _cloudStorageAccount.CreateCloudTableClient();

            notificationTable = tableClient.GetTableReference("NotificationTable");
            notificationTable.CreateIfNotExists();

            healthCheckTable = tableClient.GetTableReference("HealthCheck");
            healthCheckTable.CreateIfNotExists();

            /*_smtpServer = RoleEnvironment.GetConfigurationSettingValue("SmtpServer");
            _smtpPort = int.Parse(RoleEnvironment.GetConfigurationSettingValue("SmtpPort"));
            _smtpUser = RoleEnvironment.GetConfigurationSettingValue("SmtpUser");
            _smtpPassword = RoleEnvironment.GetConfigurationSettingValue("SmtpPassword");*/

            Trace.TraceInformation("NotificationServiceWorker has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("NotificationServiceWorker is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("NotificationServiceWorker has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");

                try
                {
                    CloudQueueMessage message = await _queue.GetMessageAsync();
                    if (message != null)
                    {
                        string commentId = message.AsString;
                        Comment comment = _commentRepository.GetComment(commentId);
                        if (comment != null)
                        {
                            List<string> subscribersEmails = GetSubscribersEmails(comment.TopicId);
                            await SendEmailsAsync(subscribersEmails, comment.Text);
                            PersistNotificationLog(commentId, subscribersEmails.Count);
                            await _queue.DeleteMessageAsync(message);
                        }
                    }
                    else
                    {
                        await Task.Delay(1000);
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Error in processing queue: " + ex.Message);
                    await Task.Delay(5000);
                }
            }
        }

        private List<string> GetSubscribersEmails(int topicId)
        {
            var subscriptions = _subscriptionRepository.RetrieveAllSubscriptions()
                                                       .Where(sub => sub.Topic_id == topicId)
                                                       .ToList();
            return subscriptions.Select(sub => GetEmailByUserId(sub.User_id)).ToList();
        }

        private string GetEmailByUserId(int userId)
        {
            var user = _userRepository.GetUser(userId.ToString());
            return user?.Email ?? "user@example.com";
        }

        private async Task SendEmailsAsync(List<string> emails, string commentText)
        {
            var fromAddress = new MailAddress("noreply@example.com", "Forum Notifications");
            var subject = "New Comment Notification";
            var plainTextContent = commentText;
            var htmlContent = $"<p>{commentText}</p>";

            using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
            {
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);

                var tasks = emails.Select(email =>
                {
                    var toAddress = new MailAddress(email);
                    var mailMessage = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = htmlContent,
                        IsBodyHtml = true,
                    };
                    return smtpClient.SendMailAsync(mailMessage);
                });

                var notificationEntity = new NotificationEntity(DateTime.UtcNow.ToString("yyyyMMdd"), Guid.NewGuid().ToString())
                {
                    CommentId = "your_comment_id",
                    SentDateTime = DateTime.UtcNow,
                    NumberOfEmailsSent = emails.Count
                };

                TableOperation insertOperation = TableOperation.Insert(notificationEntity);
                notificationTable.Execute(insertOperation);

                await Task.WhenAll(tasks);
            }
        }

        private void PersistNotificationLog(string commentId, int emailsSent)
        {
            var log = new NotificationLogEntity
            {
                PartitionKey = DateTime.UtcNow.ToString("yyyyMMdd"),
                RowKey = Guid.NewGuid().ToString(),
                CommentId = commentId,
                DateSent = DateTime.UtcNow,
                EmailsSent = emailsSent
            };

            TableOperation insertOperation = TableOperation.Insert(log);
            notificationTable.Execute(insertOperation);
        }

        private async void StartHealthCheckServer()
        {
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                foreach (var url in _healthCheckUrls)
                {
                    bool isHealthy = await CheckHealthAsync(url);
                    await PersistHealthCheckResult(url, isHealthy);

                    /*if (!isHealthy)
                    {
                        await SendHealthAlertAsync(url);
                    }*/
                }

                await Task.Delay(TimeSpan.FromSeconds(5), cancellationTokenSource.Token);
            }
        }

        private async Task<bool> CheckHealthAsync(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Health check failed: " + ex.Message);
                return false;
            }
        }

        private async Task PersistHealthCheckResult(string url, bool isHealthy)
        {
            var log = new HealthCheck(url, DateTime.UtcNow)
            {
                Status = isHealthy ? "OK" : "NOT_OK"
            };

            TableOperation insertOperation = TableOperation.Insert(log);
            await healthCheckTable.ExecuteAsync(insertOperation);
        }


        private async Task SendHealthAlertAsync(string url)
        {
            var fromAddress = new MailAddress("noreply@example.com", "Health Monitoring");
            var subject = "Service Health Alert";
            var plainTextContent = $"Health check failed for {url} at {DateTime.UtcNow}.";
            var htmlContent = $"<p>Health check failed for {url} at {DateTime.UtcNow}.</p>";

            using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
            {
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);

                var tasks = _alertEmails.Select(email =>
                {
                    var toAddress = new MailAddress(email);
                    var mailMessage = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = htmlContent,
                        IsBodyHtml = true,
                    };
                    return smtpClient.SendMailAsync(mailMessage);
                });

                await Task.WhenAll(tasks);
            }
        }
    }
}