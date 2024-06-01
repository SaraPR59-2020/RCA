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
    public class TopicDataRepository
    {
        private CloudTable _cloudTable;
        private CloudStorageAccount _cloudStorageAccount;

        public TopicDataRepository()
        {
            _cloudStorageAccount =
            CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new
            Uri(_cloudStorageAccount.TableEndpoint.AbsoluteUri), _cloudStorageAccount.Credentials);
            _cloudTable = tableClient.GetTableReference("TopicTable");
            _cloudTable.CreateIfNotExists();
        }
        public IQueryable<Topic> RetrieveAllTopics()
        {
            var results = from g in _cloudTable.CreateQuery<Topic>()
                          where g.PartitionKey == "Topic"
                          select g;
            return results;
        }
        public void AddTopic(Topic newTopic)
        {
            TableOperation insertOperation = TableOperation.Insert(newTopic);
            _cloudTable.Execute(insertOperation);
        }

        public Topic GetTopic(string index)
        {
            return RetrieveAllTopics().Where(p => p.RowKey == index).FirstOrDefault();
        }

        public void RemoveTopic(string id)
        {
            Topic topic = RetrieveAllTopics().Where(s => s.RowKey == id).FirstOrDefault();

            if (topic != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(topic);
                _cloudTable.Execute(deleteOperation);
            }
        }

        public void UpdateTopic(Topic topic)
        {
            TableOperation updateOperation = TableOperation.Replace(topic);
            _cloudTable.Execute(updateOperation);
        }
    }
}
