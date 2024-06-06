using System;
using Common;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System.Net;
using Microsoft.Azure;

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
        private List<string> _healthCheckUrls = new List<string>
        {
            "http://redditservice/health-monitoring",
            "http://notificationservice/health-monitoring"
        };
        private List<string> _alertEmails = new List<string> { "admin@example.com" };

        public override void Run()
        {
            Trace.TraceInformation("NotificationServiceWorker is running");

            // Start the health check server in a new thread
            Task.Run(() => StartHealthCheckServer(), cancellationTokenSource.Token);

            try
            {
                RunAsync(cancellationTokenSource.Token).Wait();
            }
            finally
            {
                runCompleteEvent.Set();
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

            Trace.TraceInformation("NotificationServiceWorker has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("NotificationServiceWorker is stopping");

            cancellationTokenSource.Cancel();
            runCompleteEvent.WaitOne();

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
                    var message = await _queue.GetMessageAsync();
                    if (message != null)
                    {
                        string commentId = message.AsString;
                        var comment = _commentRepository.GetComment(commentId);
                        if (comment != null)
                        {
                            var subscribersEmails = GetSubscribersEmails(comment.TopicId);
                            PersistNotificationLog(commentId, subscribersEmails.Count);
                            await _queue.DeleteMessageAsync(message);
                        }
                    }
                    else
                    {
                        await Task.Delay(1000, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Error in processing queue: " + ex.Message);
                    await Task.Delay(5000, cancellationToken);
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

            var insertOperation = TableOperation.Insert(log);
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

            var insertOperation = TableOperation.Insert(log);
            await healthCheckTable.ExecuteAsync(insertOperation);
        }
    }
    public class HealthCheck : TableEntity
    {
        public HealthCheck(string serviceName, DateTime checkTime)
        {
            PartitionKey = checkTime.ToString("yyyyMMdd");
            RowKey = Guid.NewGuid().ToString();
            ServiceName = serviceName;
            CheckTime = checkTime;
        }

        public string ServiceName { get; set; }
        public DateTime CheckTime { get; set; }
        public string Status { get; set; }
    }
}
