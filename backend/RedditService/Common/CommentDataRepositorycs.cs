using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure;
using Microsoft.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class CommentDataRepositorycs
    {
        private CloudTable _cloudTable;
        private CloudStorageAccount _cloudStorageAccount;
        public CommentDataRepositorycs()
        {
            _cloudStorageAccount =
            CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new
            Uri(_cloudStorageAccount.TableEndpoint.AbsoluteUri), _cloudStorageAccount.Credentials);
            _cloudTable = tableClient.GetTableReference("CommentTable");
            _cloudTable.CreateIfNotExists();
        }
        public IQueryable<Comment> RetrieveAllCommentss()
        {
            var results = from g in _cloudTable.CreateQuery<Comment>()
                          where g.PartitionKey == "Comment"
                          select g;
            return results;
        }
        public void AddComment(Comment newComment)
        {
            // Samostalni rad: izmestiti tableName u konfiguraciju servisa.
            TableOperation insertOperation = TableOperation.Insert(newComment);
            _cloudTable.Execute(insertOperation);
        }

        public void RemoveComment(string id)
        {
            Comment comment = RetrieveAllCommentss().Where(s => s.RowKey == id).FirstOrDefault();

            if (comment != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(comment);
                _cloudTable.Execute(deleteOperation);
            }
        }

        public Comment GetComment(string index)
        {
            return RetrieveAllCommentss().Where(p => p.RowKey == index).FirstOrDefault();
        }
    }
}
