using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using NotificationServiceWorker;

public class NotificationController : Controller
{
    private CloudTable notificationsLogTable;

    public NotificationController()
    {
        
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
        CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
        notificationsLogTable = tableClient.GetTableReference("NotificationTable");
        notificationsLogTable.CreateIfNotExists();
    }

    public ActionResult Index()
    {
        var logs = GetNotificationLogs();
        return View(logs);
    }

    private List<NotificationLogEntity> GetNotificationLogs()
    {
        var query = new TableQuery<NotificationLogEntity>();
        return notificationsLogTable.ExecuteQuery(query).ToList();
    }
}
