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
using Postmark;
using PostmarkDotNet;

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
                await CheckHealthAsync();
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
            return await SendRequestToService("http://RedditServiceWeb/HealthMonitoringServiceEndpoint");
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
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string postmarkServerToken = CloudConfigurationManager.GetSetting("PostmarkServerToken");
            string fromEmail = CloudConfigurationManager.GetSetting("FromEmail");  // Ensure this is a valid email address
            string toEmail = CloudConfigurationManager.GetSetting("ToEmail");  // Ensure this is a valid email address

            // Log configuration details
            Trace.TraceInformation($"Postmark Server Token: {postmarkServerToken}, From: {fromEmail}, To: {toEmail}");

            try
            {
                
                var client = new PostmarkClient(postmarkServerToken);

                var message = new PostmarkMessage
                {
                    From = fromEmail,
                    To = toEmail,
                    Subject = $"Health check failed for {serviceName}",
                    TextBody = $"The health check for {serviceName} failed at {DateTime.UtcNow}"
                };

                var sendResult = await client.SendMessageAsync(message);

                if (sendResult.Status != PostmarkStatus.Success)
                {
                    Trace.TraceError($"Postmark error: {sendResult.Message}");
                    throw new Exception($"Failed to send email via Postmark: {sendResult.Message}");
                }
            }
            catch (FormatException ex)
            {
                Trace.TraceError($"Email format error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Unexpected error: {ex.Message}");
                throw;
            }
        }
    }

}
