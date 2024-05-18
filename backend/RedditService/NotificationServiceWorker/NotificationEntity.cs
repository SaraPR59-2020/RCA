using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;


namespace NotificationServiceWorker
{
    public class NotificationEntity : TableEntity
    {
        public string CommentId { get; set; }
        public DateTime SentDateTime { get; set; }
        public int NumberOfEmailsSent { get; set; }

        public NotificationEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public NotificationEntity() { }
    }
}
