using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Configuration;
using RedditServiceWeb.Models;

namespace RedditServiceWeb
{
    public class RedditService
    {
        private readonly CloudQueue _queue;

        public RedditService()
        {
            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            //CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            //_queue = queueClient.GetQueueReference("notifications");
            //_queue.CreateIfNotExists();
        }

        public void PostComment(Comment comment)
        {
            // Kod za postavljanje komentara...

            // Slanje poruke u red
            CloudQueueMessage message = new CloudQueueMessage(comment.Id.ToString());
            _queue.AddMessage(message);
        }
    }
}
