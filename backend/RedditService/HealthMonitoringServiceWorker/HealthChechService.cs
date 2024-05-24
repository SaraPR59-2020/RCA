using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HealthMonitoringServiceWorker
{
    public class HealthCheckService
    {
        private readonly WorkerRole workerRole;

        public HealthCheckService()
        {
            workerRole = new WorkerRole();
        }

        public async Task<string> CheckHealth()
        {
            bool redditServiceIsHealthy = await workerRole.SendRequestToRedditService();
            bool notificationServiceIsHealthy = await workerRole.SendRequestToNotificationService();

            string result = $"RedditService: {(redditServiceIsHealthy ? "Healthy" : "Unhealthy")}, " +
                            $"NotificationService: {(notificationServiceIsHealthy ? "Healthy" : "Unhealthy")}";

            return result;
        }
    }
}

