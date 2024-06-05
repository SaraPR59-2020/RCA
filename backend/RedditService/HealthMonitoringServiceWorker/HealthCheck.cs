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
        public HealthCheck(string serviceName, DateTime checkTime)
        {
            PartitionKey = checkTime.ToString("yyyyMMdd");
            RowKey = Guid.NewGuid().ToString();
            ServiceName = serviceName;
            CheckTime = checkTime;
        }

        public string ServiceName { get; set; }
        public DateTime CheckTime { get; set; }
        public string Status { get; set; }
    }

}
