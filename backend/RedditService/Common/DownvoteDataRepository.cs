using Microsoft.Azure;
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
    public class DownvoteDataRepository
    {
        private CloudTable _cloudTable;
        private CloudStorageAccount _cloudStorageAccount;

        public DownvoteDataRepository()
        {
            _cloudStorageAccount =
            CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new
            Uri(_cloudStorageAccount.TableEndpoint.AbsoluteUri), _cloudStorageAccount.Credentials);
            _cloudTable = tableClient.GetTableReference("DownvoteTable");
            _cloudTable.CreateIfNotExists();
        }
        public IQueryable<Downvote> RetrieveAllDownvotes()
        {
            var results = from g in _cloudTable.CreateQuery<Downvote>()
                          where g.PartitionKey == "Downvote"
                          select g;
            return results;
        }
        public void AddDownvote(Downvote newDownvote)
        {
            TableOperation insertOperation = TableOperation.Insert(newDownvote);
            _cloudTable.Execute(insertOperation);
        }

        public Downvote GetDownvote(string index)
        {
            return RetrieveAllDownvotes().Where(p => p.RowKey == index).FirstOrDefault();
        }

        public void UpdateDownvote(Downvote downvote)
        {
            TableOperation updateOperation = TableOperation.Replace(downvote);
            _cloudTable.Execute(updateOperation);
        }

        public List<int> GetTopicByUser(int user_id)
        {
            List<Downvote> selectedDownvotes = RetrieveAllDownvotes().Where(p => p.User_id == user_id).ToList();
            List<int> topic = new List<int>();
            foreach (Downvote d in selectedDownvotes)
            {
                topic.Add(d.Topic_id);
            }
            return topic;
        }

        public void RemoveDownvote(string id)
        {
            Downvote downvote = RetrieveAllDownvotes().Where(s => s.RowKey == id).FirstOrDefault();

            if (downvote != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(downvote);
                _cloudTable.Execute(deleteOperation);
            }
        }
    }
}
