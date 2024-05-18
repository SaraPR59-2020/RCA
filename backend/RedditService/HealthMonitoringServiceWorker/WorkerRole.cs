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

using System.Timers;
using Microsoft.Extensions.Logging;
using System.Net.Http;


namespace HealthMonitoringServiceWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

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
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

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

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }


        public static void RunTimerTrigger(ILogger log)
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 5000; // Interval u milisekundama (ovde 5 sekundi)
            timer.Elapsed += async (sender, e) =>
            {
                    // Slanje zahteva ka RedditService i NotificationService
            };
            timer.Start();

            // Sačekaj da se funkcija ne završi odmah
            Thread.Sleep(Timeout.Infinite);
        }


        private static async Task<HttpResponseMessage> SendRequestToRedditService()
        {
            HttpResponseMessage response = null;

            // Logika slanja zahteva i dobijanja odgovora

            return response;
        }

        private static async Task<HttpResponseMessage> SendRequestToNotificationService()
        {
            HttpResponseMessage response = null;

            // Logika slanja zahteva i dobijanja odgovora

            return response;
        }

    }
}
