using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Downvote : TableEntity
    {
        private int downvote_id;
        private int user_id;
        private int topic_id;

        public int Downvote_id { get => downvote_id; set => downvote_id = value; }
        public int User_id { get => user_id; set => user_id = value; }
        public int Topic_id { get => topic_id; set => topic_id = value; }

        public Downvote(int downvote_id, int user_id, int topic_id)
        {
            this.downvote_id = downvote_id;
            this.user_id = user_id;
            this.topic_id = topic_id;
        }

        public Downvote(string indexNo)
        {
            PartitionKey = "Downvote";
            RowKey = indexNo;
        }
        public Downvote()
        {

        }
    }
}
