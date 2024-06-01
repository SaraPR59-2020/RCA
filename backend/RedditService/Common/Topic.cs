using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class Topic : TableEntity
    {
        private int topic_id;
        private string headline;
        private string content;
        private int user;
        private int upvote_number;
        private int downvote_number;
        private int comment_number;
        private string topic_image;

        public Topic(int topic_id, string headline, string content, int user, int upvote_number, int downvote_number, int comment_number, string topic_image)
        {
            this.topic_id = topic_id;
            this.headline = headline;
            this.content = content;
            this.user = user;
            this.upvote_number = upvote_number;
            this.downvote_number = downvote_number;
            this.comment_number = comment_number;
            this.topic_image = topic_image;
        }
       
        public int Topic_id { get => topic_id; set => topic_id = value; }
        public string Headline { get => headline; set => headline = value; }
        public string Content { get => content; set => content = value; }
        public int User { get => user; set => user = value; }
        public int Upvote_number { get => upvote_number; set => upvote_number = value; }
        public int Downvote_number { get => downvote_number; set => downvote_number = value; }
        public int Comment_number { get => comment_number; set => comment_number = value; }
        public string Topic_image { get => topic_image; set => topic_image = value; }

        public Topic(string indexNo)
        {
            PartitionKey = "Topic";
            RowKey = indexNo;
        }
        public Topic()
        {

        }
    }
}
