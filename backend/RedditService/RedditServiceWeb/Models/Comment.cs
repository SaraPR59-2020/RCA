using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedditServiceWeb.Models
{

    public class Comment
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string TopicId { get; set; }
    }
}