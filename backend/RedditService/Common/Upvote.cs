using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Upvote : TableEntity
    {
        private int upvote_id;
        private int user_id;
        private int topic_id;

        public int Upvote_id { get => upvote_id; set => upvote_id = value; }
        public int User_id { get => user_id; set => user_id = value; }
        public int Topic_id { get => topic_id; set => topic_id = value; }

        public Upvote(int upvote_id, int user_id, int topic_id)
        {
            this.upvote_id = upvote_id;
            this.user_id = user_id;
            this.topic_id = topic_id;
        }

        public Upvote(string indexNo)
        {
            PartitionKey = "Upvote";
            RowKey = indexNo;
        }
        public Upvote()
        {

        }
    }
}
