using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationServiceWorker
{
    public class NotificationLogEntity : TableEntity
    {
        public string CommentId { get; set; }
        public DateTime DateSent { get; set; }
        public int EmailsSent { get; set; }
    }
}
