using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringServiceWorker
{
    public class HealthCheck
    {
            public int Id { get; set; }
            public string ServiceName { get; set; }
            public DateTime CheckTime { get; set; }
            public string Status { get; set; }

    }
}
