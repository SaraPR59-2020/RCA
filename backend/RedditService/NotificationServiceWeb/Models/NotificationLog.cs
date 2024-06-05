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
        public DateTime DateSent { get; set; }
        public int EmailsSent { get; set; }
    }
}