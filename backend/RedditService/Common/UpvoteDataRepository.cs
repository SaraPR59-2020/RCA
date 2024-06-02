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
    public class UpvoteDataRepository
    {
        private CloudTable _cloudTable;
        private CloudStorageAccount _cloudStorageAccount;

        public UpvoteDataRepository()
        {
            _cloudStorageAccount =
            CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new
            Uri(_cloudStorageAccount.TableEndpoint.AbsoluteUri), _cloudStorageAccount.Credentials);
            _cloudTable = tableClient.GetTableReference("UpvoteTable");
            _cloudTable.CreateIfNotExists();
        }
        public IQueryable<Upvote> RetrieveAllUpvotes()
        {
            var results = from g in _cloudTable.CreateQuery<Upvote>()
                          where g.PartitionKey == "Upvote"
                          select g;
            return results;
        }
        public void AddUpvote(Upvote newUpvote)
        {
            TableOperation insertOperation = TableOperation.Insert(newUpvote);
            _cloudTable.Execute(insertOperation);
        }

        public Upvote GetUpvote(string index)
        {
            return RetrieveAllUpvotes().Where(p => p.RowKey == index).FirstOrDefault();
        }

        public void UpdateUpvote(Upvote upvote)
        {
            TableOperation updateOperation = TableOperation.Replace(upvote);
            _cloudTable.Execute(updateOperation);
        }

        public List<int> GetTopicByUser(int user_id)
        {
            List<Upvote> selectedUpvotes = RetrieveAllUpvotes().Where(p => p.User_id == user_id).ToList();
            List<int> topic = new List<int>();
            foreach (Upvote u in selectedUpvotes)
            {
                topic.Add(u.Topic_id);
            }
            return topic;
        }

        public void RemoveUpvote(string id)
        {
            Upvote upvote = RetrieveAllUpvotes().Where(s => s.RowKey == id).FirstOrDefault();

            if (upvote != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(upvote);
                _cloudTable.Execute(deleteOperation);
            }
        }
    }
}
