using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace NotificationServiceWorker
{
    public class NotificationLogEntity : TableEntity
    {
        public string CommentId { get; set; }
        public DateTime DateSent { get; set; }
        public int EmailsSent { get; set; }
    }
}
