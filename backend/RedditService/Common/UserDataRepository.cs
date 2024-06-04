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
    public class UserDataRepository
    {
        private CloudTable _cloudTable;
        private CloudStorageAccount _cloudStorageAccount;

        public UserDataRepository()
        {
            _cloudStorageAccount =
            CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new
            Uri(_cloudStorageAccount.TableEndpoint.AbsoluteUri), _cloudStorageAccount.Credentials);
            _cloudTable = tableClient.GetTableReference("UserTable");
            _cloudTable.CreateIfNotExists();
        }
        public IQueryable<User> RetrieveAllUsers()
        {
            var results = from g in _cloudTable.CreateQuery<User>()
                          where g.PartitionKey == "User"
                          select g;
            return results;
        }
        public void AddUser(User newUser)
        {
            TableOperation insertOperation = TableOperation.Insert(newUser);
            _cloudTable.Execute(insertOperation);
        }

        public User GetUser(string index)
        {
            return RetrieveAllUsers().Where(p => p.RowKey == index).FirstOrDefault();
        }

        public void UpdateUser(User user)
        {
            TableOperation updateOperation = TableOperation.Replace(user);
            _cloudTable.Execute(updateOperation);
        }
    }
}
