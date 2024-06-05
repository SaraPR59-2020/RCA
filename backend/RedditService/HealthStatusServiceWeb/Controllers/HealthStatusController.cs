using HealthStatusServiceWeb.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HealthStatusServiceWeb.Controllers
{
    public class HealthStatusController : Controller
    {
        private CloudTable healthCheckTable;

        public HealthStatusController()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            healthCheckTable = tableClient.GetTableReference("HealthCheck");
            healthCheckTable.CreateIfNotExists();
        }

        public ActionResult Index()
        {
            var checks = GetHealthChecksLastHour();
            var uptime = CalculateUptime(checks);
            var model = new HealthStatus
            {
                UptimeLast24Hours = uptime
            };
            return View(model);
        }

        private List<HealthCheck> GetHealthChecksLastHour()
        {
            var query = new TableQuery<HealthCheck>().Where(
                TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.GreaterThanOrEqual, DateTime.UtcNow.AddHours(-1)));
            return healthCheckTable.ExecuteQuery(query).ToList();
        }

        private double CalculateUptime(List<HealthCheck> checks)
        {
            int totalChecks = checks.Count;
            int successfulChecks = checks.Count(c => c.Status == "OK");
            return totalChecks > 0 ? (double)successfulChecks / totalChecks * 100 : 0;
        }
    }
}