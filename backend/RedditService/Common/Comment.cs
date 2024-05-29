using System;
using System.Collections.Generic;
using System.Linq;
public class Comment
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
}

