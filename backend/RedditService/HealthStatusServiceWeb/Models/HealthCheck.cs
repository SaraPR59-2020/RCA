using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthStatusServiceWeb.Models
{
    public class HealthCheck: TableEntity
    {
        public string ServiceName { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }

        public HealthCheck() { }

        public HealthCheck(string serviceName, string status, DateTime timestamp)
        {
            PartitionKey = serviceName;
            RowKey = Guid.NewGuid().ToString();
            ServiceName = serviceName;
            Status = status;
            Timestamp = timestamp;
        }
    }
}