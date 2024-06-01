using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common;

namespace RedditServiceWeb
{
    public class HealthMonitoring : IHealthMonitoring
    {
        public void IAmAlive()
        {
            Console.WriteLine("Worker role has invoked service");
        }
    }
}