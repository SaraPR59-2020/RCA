using Microsoft.Azure;
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
        private static CloudTable healthCheckTable;

        static HealthStatusController()
        {
            try
            {
                CloudStorageAccount _cloudStorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString")); ;
                CloudTableClient tableClient = _cloudStorageAccount.CreateCloudTableClient();
                healthCheckTable = tableClient.GetTableReference("HealthCheck");
                healthCheckTable.CreateIfNotExists();
            }
            catch (Exception ex)
            {
                // Log or handle the exception as necessary
                throw new InvalidOperationException("Unable to initialize Azure table storage", ex);
            }
        }

        public ActionResult Index()
        {
            try
            {
                var checks = GetHealthChecksLastHour();
                var uptime = CalculateUptime(checks);
                var model = new HealthStatus
                {
                    UptimeLast24Hours = uptime
                };
                return View(model);
            }
            catch (Exception ex)
            {
                // Log or handle the exception as necessary
                return new HttpStatusCodeResult(500, "Internal server error");
            }
        }

        private List<HealthCheck> GetHealthChecksLastHour()
        {
            var query = new TableQuery<HealthCheck>().Where(
                TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.GreaterThanOrEqual, DateTimeOffset.UtcNow.AddHours(-1)));
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
