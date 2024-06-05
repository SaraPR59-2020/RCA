using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using System.ServiceModel;
using Common;
using System.Timers;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace HealthMonitoringServiceWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private CloudTable healthCheckTable;

        public override void Run()
        {
            Trace.TraceInformation("HealthMonitoringServiceWorker is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;
            bool result = base.OnStart();
            Connect();
            Trace.TraceInformation("HealthMonitoringServiceWorker has been started");
            return result;

        }

        public override void OnStop()
        {
            Trace.TraceInformation("HealthMonitoringServiceWorker is stopping");
            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();
            base.OnStop();
            Trace.TraceInformation("HealthMonitoringServiceWorker has stopped");
        }

        private IHealthMonitoring proxy;
        public void Connect()
        {
            var binding = new NetTcpBinding();
            ChannelFactory<IHealthMonitoring> factory = new
            ChannelFactory<IHealthMonitoring>(binding, new
            EndpointAddress("net.tcp://localhost:6000/HealthMonitoring"));
            proxy = factory.CreateChannel();
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 5000; // 5 sekundi
            timer.Elapsed += async (sender, e) =>
            {
                proxy.IAmAlive();
                //await CheckHealthAsync();     ovde puca
            };
            timer.Start();
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }

        private async Task CheckHealthAsync()
        {
            bool redditServiceIsHealthy = await SendRequestToRedditService();
            await LogHealthCheckAsync("RedditService", redditServiceIsHealthy);

            if (!redditServiceIsHealthy)
            {
                await SendFailureEmailAsync("RedditService");
            }

            bool notificationServiceIsHealthy = await SendRequestToNotificationService();
            await LogHealthCheckAsync("NotificationService", notificationServiceIsHealthy);

            if (!notificationServiceIsHealthy)
            {
                await SendFailureEmailAsync("NotificationService");
            }
        }

        private static async Task<bool> SendRequestToService(string serviceUrl)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(serviceUrl);
                    return response.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SendRequestToRedditService()
        {
            return await SendRequestToService("http://RedditService/");
        }

        public async Task<bool> SendRequestToNotificationService()
        {
            return await SendRequestToService("http://NotificationServiceWorker/HealthMonitoringServiceEndpoint");
        }

        private async Task LogHealthCheckAsync(string serviceName, bool isSuccess)
        {
            var _cloudStorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            var tableClient = _cloudStorageAccount.CreateCloudTableClient();
            healthCheckTable = tableClient.GetTableReference("HealthCheck");
            await healthCheckTable.CreateIfNotExistsAsync();

            var healthCheck = new HealthCheck(serviceName, DateTime.UtcNow)
            {
                Status = isSuccess ? "OK" : "NOT_OK"
            };

            var insertOperation = TableOperation.Insert(healthCheck);
            await healthCheckTable.ExecuteAsync(insertOperation);
        }


        private async Task SendFailureEmailAsync(string serviceName)
        {
            string toEmail = "configured-email@example.com"; // Nabaviti iz konfiguracije
            string subject = $"Health check failed for {serviceName}";
            string body = $"The health check for {serviceName} failed at {DateTime.UtcNow}";

            using (System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient("smtp.example.com"))
            {
                client.Credentials = new NetworkCredential("username", "password");
                client.EnableSsl = true;
                await client.SendMailAsync("noreply@example.com", toEmail, subject, body);
            }
        }
    }

}
