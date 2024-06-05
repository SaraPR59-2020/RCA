using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotificationServiceWeb.Models
{
    public class NotificationLog: TableEntity
    {
        public string CommentId { get; set; }
        public int EmailsSent { get; set; }
        public DateTime Timestamp { get; set; }

        public NotificationLog() { }

        public NotificationLog(string commentId, int emailsSent, DateTime timestamp)
        {
            PartitionKey = commentId;
            RowKey = Guid.NewGuid().ToString();
            CommentId = commentId;
            EmailsSent = emailsSent;
            Timestamp = timestamp;
        }
    }
}