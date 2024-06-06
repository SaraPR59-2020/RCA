using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditServiceWeb;
using HealthMonitoringServiceWorker;

namespace AdminToolsConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Admin Tools Console App");



            // Primer korišćenja funkcionalnosti iz HealthMonitoringServiceWorker
            var healthService = new HealthCheckService();
            var healthStatus = healthService.CheckHealth();
            Console.WriteLine($"Health Status: {healthStatus}");


        }
    }
}


