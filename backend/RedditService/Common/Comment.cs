using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;
public class Comment : TableEntity
{
    public int Id { get; set; }
    public string Text { get; set; }
    public int TopicId { get; set; }
    public int UserId { get; set; }

    public Comment(int id, string text, int topicId, int userId)
    {
        Id = id;
        Text = text;
        TopicId = topicId;
        UserId = userId;
    }
    public Comment (string indexNo)
    {
        PartitionKey = "Comment";
        RowKey = indexNo;
    }
    public Comment()
    {

    }
}

