using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class SubscriptionDataRepository
    {
        private CloudTable _cloudTable;
        private CloudStorageAccount _cloudStorageAccount;

        public SubscriptionDataRepository()
        {
            _cloudStorageAccount =
            CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new
            Uri(_cloudStorageAccount.TableEndpoint.AbsoluteUri), _cloudStorageAccount.Credentials);
            _cloudTable = tableClient.GetTableReference("SubscriptionTable");
            _cloudTable.CreateIfNotExists();
        }
        public IQueryable<Subscription> RetrieveAllSubscriptions()
        {
            var results = from g in _cloudTable.CreateQuery<Subscription>()
                          where g.PartitionKey == "Subscription"
                          select g;
            return results;
        }
        public void AddSubscription(Subscription newSubscription)
        {
            TableOperation insertOperation = TableOperation.Insert(newSubscription);
            _cloudTable.Execute(insertOperation);
        }

        public Subscription GetSubscription(string index)
        {
            return RetrieveAllSubscriptions().Where(p => p.RowKey == index).FirstOrDefault();
        }

        public void UpdateSubscription(Subscription subscription)
        {
            TableOperation updateOperation = TableOperation.Replace(subscription);
            _cloudTable.Execute(updateOperation);
        }

        public List<int> GetTopicByUser(int user_id)
        {
            List<Subscription> selectedSubscriptions = RetrieveAllSubscriptions().Where(p => p.User_id == user_id).ToList();
            List<int> topic = new List<int>();
            foreach (Subscription s in selectedSubscriptions)
            {
                topic.Add(s.Topic_id);
            }
            return topic;
        }

        public void RemoveSubscription(string id)
        {
            Subscription subscription = RetrieveAllSubscriptions().Where(s => s.RowKey == id).FirstOrDefault();

            if (subscription != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(subscription);
                _cloudTable.Execute(deleteOperation);
            }
        }
    }
}
