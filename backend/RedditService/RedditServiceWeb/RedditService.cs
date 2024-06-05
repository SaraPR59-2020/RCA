using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Configuration;
using RedditServiceWeb.Models;
using Microsoft.Azure;

namespace RedditServiceWeb
{
    public class RedditService
    {
        private readonly CloudQueue _queue;

        public RedditService()
        {
            var _cloudStorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            var tableClient = _cloudStorageAccount.CreateCloudTableClient();
            CloudQueueClient queueClient = _cloudStorageAccount.CreateCloudQueueClient();
            _queue = queueClient.GetQueueReference("notifications");
            _queue.CreateIfNotExists();
        }

        public void PostComment(Comment comment)
        {
            // Kod za postavljanje komentara je u funkciji koja poziva ovu
            CloudQueueMessage message = new CloudQueueMessage(comment.Id.ToString());
            _queue.AddMessage(message);
        }
    }
}
