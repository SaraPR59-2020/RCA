using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
//send grid using
using SendGrid;
using SendGrid.Helpers.Mail;



namespace NotificationServiceWorker
{
    
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private CloudQueue _queue;
        private CloudTable _table;
        protected CloudTable notificationTable;
        private string _sendGridApiKey;

        public override void Run()
        {
            Trace.TraceInformation("NotificationServiceWorker is running");

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
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            bool result = base.OnStart();

            // Initialize storage account
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            _queue = queueClient.GetQueueReference("notifications");
            _queue.CreateIfNotExists();

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference("Subscriptions");
            _table.CreateIfNotExists();

            // Get a reference to the table you want to create (NotificationTable)
            notificationTable = tableClient.GetTableReference("NotificationTable");

            // Create the table if it doesn't exist
            notificationTable.CreateIfNotExists();

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

                        // Dohvatanje komentara i pretplaćenih korisnika
                        Comment comment = GetCommentById(commentId);
                        List<string> subscribersEmails = GetSubscribersEmails(comment.TopicId);

                        // Slanje mejlova
                        await SendEmailsAsync(subscribersEmails, comment.Text);

                        // Persistovanje informacije o poslatim notifikacijama
                        PersistNotificationLog(commentId, subscribersEmails.Count);

                        await _queue.DeleteMessageAsync(message);
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

        private Comment GetCommentById(string commentId)
        {
            // Implementacija metode za dohvatanje komentara po ID-ju
            // Ovo je mock implementacija
            return new Comment { Id = commentId, Text = "Example comment text", TopicId = "1" };
        }

        private List<string> GetSubscribersEmails(string topicId)
        {
            // Implementacija metode za dohvatanje mejlova pretplaćenih korisnika
            // Ovo je mock implementacija
            return new List<string> { "user1@example.com", "user2@example.com" };
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
            _table.Execute(insertOperation);
        }

        public class NotificationLogEntity : TableEntity
        {
            public string CommentId { get; set; }
            public DateTime DateSent { get; set; }
            public int EmailsSent { get; set; }
        }

        public class Comment
        {
            public string Id { get; set; }
            public string Text { get; set; }
            public string TopicId { get; set; }
        }
    }

}
