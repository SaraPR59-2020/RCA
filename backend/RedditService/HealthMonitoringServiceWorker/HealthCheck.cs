using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringServiceWorker
{
    public class HealthCheck : TableEntity
    {
            public int Id { get; set; }
            public string ServiceName { get; set; }
            public DateTime CheckTime { get; set; }
            public string Status { get; set; }

        public HealthCheck()
        {
        }

        public HealthCheck(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

    }
}
