using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Common;
using HealthMonitoringServiceWorker;
using Microsoft.Azure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using SendGrid;
using SendGrid.Helpers.Mail;

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
        private string _sendGridApiKey;
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

        //private void StartHealthCheckServer()
       // {
            //HttpListener listener = new HttpListener();
            //listener.Prefixes.Add("http://*:8081/health-monitoring/");
            //listener.Start();   //acces is denied greska pise 
            //Trace.TraceInformation("Health check server is running at http://localhost:8081/health-monitoring/");

            //while (true)
            //{
            //    HttpListenerContext context = listener.GetContext();
            //    HttpListenerResponse response = context.Response;
            //    string responseString = "NotificationService is healthy";
            //    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            //    response.ContentLength64 = buffer.Length;
            //    System.IO.Stream output = response.OutputStream;
            //    output.Write(buffer, 0, buffer.Length);
            //    output.Close();
            //}
      //  }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            bool result = base.OnStart();
             /*_cloudStorageAccount =
             CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
             CloudTableClient tableClient = new CloudTableClient(new
             Uri(_cloudStorageAccount.TableEndpoint.AbsoluteUri), _cloudStorageAccount.Credentials);
             _cloudTable = tableClient.GetTableReference("Subscriptions");
             _cloudTable.CreateIfNotExists();*/
           var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            var queueClient = _cloudStorageAccount.CreateCloudQueueClient();
            _queue = queueClient.GetQueueReference("notifications");
            _queue.CreateIfNotExists();

            _commentRepository = new CommentDataRepositorycs();
            _subscriptionRepository = new SubscriptionDataRepository();
            _userRepository = new UserDataRepository();

            var tableClient = storageAccount.CreateCloudTableClient();


            notificationTable = tableClient.GetTableReference("NotificationTable");
            notificationTable.CreateIfNotExists();

            healthCheckTable = tableClient.GetTableReference("HealthCheck");
            healthCheckTable.CreateIfNotExists();

            _sendGridApiKey = RoleEnvironment.GetConfigurationSettingValue("SendGridApiKey");

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


                        /* Comment comment = GetCommentById(commentId);
                         List<string> subscribersEmails = GetSubscribersEmails(comment.TopicId.ToString());

                         await SendEmailsAsync(subscribersEmails, comment.Text);

                         PersistNotificationLog(commentId, subscribersEmails.Count);

                         await _queue.DeleteMessageAsync(message);*/
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

        /* private Comment GetCommentById(string commentId)
         {
             int id = 0;
             if (int.TryParse(commentId, out id))
                 return new Comment(id, "Example comment text",  1, 1 );
             else
                 return null;
         }

         private List<string> GetSubscribersEmails(string topicId)
         {
             return new List<string> { "user1@example.com", "user2@example.com" };
         }*/
        private List<string> GetSubscribersEmails(int topicId)
        {
            var subscriptions = _subscriptionRepository.RetrieveAllSubscriptions()
                                                       .Where(sub => sub.Topic_id == topicId)
                                                       .ToList();

            // Pretpostavka je da imate način da dobijete emailove korisnika po njihovim userId.
            // Ovde možete da implementirate logiku za dobijanje emailova na osnovu userId.
            return subscriptions.Select(sub => GetEmailByUserId(sub.User_id)).ToList();
        }

        private string GetEmailByUserId(int userId)
        {
            var user = _userRepository.GetUser(userId.ToString());
            if (user != null)
            {
                return user.Email;
            }
            else
            {
                // Ako korisnik nije pronađen, možete vratiti neku podrazumevanu vrednost
                return "user@example.com";
            }
        }
        private async Task SendEmailsAsync(List<string> emails, string commentText)
        {
            var client = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress("noreply@example.com", "Forum Notifications");
            var subject = "New Comment Notification";
            var plainTextContent = commentText;
            var htmlContent = $"<p>{commentText}</p>";

            var tasks = emails.Select(email =>
            {
                var to = new EmailAddress(email);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                return client.SendEmailAsync(msg);
            });

           /* var notificationEntity = new NotificationEntity(DateTime.UtcNow.ToString("yyyyMMdd"), Guid.NewGuid().ToString())
            {
                CommentId = "your_comment_id",
                SentDateTime = DateTime.UtcNow,
                NumberOfEmailsSent = emails.Count
            };

            TableOperation insertOperation = TableOperation.Insert(notificationEntity);
            notificationTable.Execute(insertOperation);*/

            await Task.WhenAll(tasks);
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

                    if (!isHealthy)
                    {
                        await SendHealthAlertAsync(url);
                    }
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
            var log = new HealthCheck
            {
                PartitionKey = DateTime.UtcNow.ToString("yyyyMMdd"),
                RowKey = Guid.NewGuid().ToString(),
                ServiceName = url,
                CheckTime = DateTime.UtcNow,
                Status = isHealthy ? "OK" : "NOT_OK"
            };

            TableOperation insertOperation = TableOperation.Insert(log);
            await healthCheckTable.ExecuteAsync(insertOperation);
        }

        private async Task SendHealthAlertAsync(string url)
        {
            var client = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress("noreply@example.com", "Health Monitoring");
            var subject = "Service Health Alert";
            var plainTextContent = $"Health check failed for {url} at {DateTime.UtcNow}.";
            var htmlContent = $"<p>Health check failed for {url} at {DateTime.UtcNow}.</p>";

            var tasks = _alertEmails.Select(email =>
            {
                var to = new EmailAddress(email);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                return client.SendEmailAsync(msg);
            });

            await Task.WhenAll(tasks);
        }
    }
}
