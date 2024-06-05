using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace HealthStatusServiceWeb.Models
{
    public class HealthCheck : TableEntity
    {
        public string ServiceName { get; set; }
        public string Status { get; set; }

        public HealthCheck() { }

        public HealthCheck(string serviceName, string status)
        {
            PartitionKey = serviceName;
            RowKey = Guid.NewGuid().ToString();
            ServiceName = serviceName;
            Status = status;
        }
    }
}
