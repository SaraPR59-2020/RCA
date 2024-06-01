using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Common;

namespace RedditServiceWeb
{
    public class WebRole : RoleEntryPoint
    {
        private ServiceHost serviceHost;
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.
            //serviceHost = new ServiceHost(typeof(HealthMonitoring));
            //NetTcpBinding binding = new NetTcpBinding();
            //serviceHost.AddServiceEndpoint(typeof(IHealthMonitoring), binding, new
            //Uri("net.tcp://localhost:6000/HealthMonitoring"));
            //serviceHost.Open();
            //Console.WriteLine("Server ready and waiting for requests.");


            return base.OnStart();
        }
    }
}
