using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Subscription : TableEntity
    {
        private int subscription_id;
        private int user_id;
        private int topic_id;

        public int Subscription_id { get => subscription_id; set => subscription_id = value; }
        public int User_id { get => user_id; set => user_id = value; }
        public int Topic_id { get => topic_id; set => topic_id = value; }

        public Subscription(int subscription_id, int user_id, int topic_id)
        {
            this.subscription_id = subscription_id;
            this.user_id = user_id;
            this.topic_id = topic_id;
        }

        public Subscription(string indexNo)
        {
            PartitionKey = "Subscription";
            RowKey = indexNo;
        }
        public Subscription()
        {

        }
    }
}
