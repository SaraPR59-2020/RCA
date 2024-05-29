using System.Web.Mvc;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

using RedditServiceWeb.Models;
using System.Collections.Generic;
using Common;
using System.Linq;

namespace RedditServiceWeb.Controllers
{
    public class ForumController : Controller
    {
        private readonly RedditService _redditService;

        public ForumController()
        {
            _redditService = new RedditService();
        }

        [HttpGet]
        public ActionResult PostComment()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PostComment(Comment comment)
        {
            if (ModelState.IsValid)
            {
                _redditService.PostComment(comment);
                // Redirekcija ili prikaz poruke o uspehu
                return RedirectToAction("Success");
            }
            return View(comment);
        }

        public ActionResult Success()
        {
            return View();
        }

        public ActionResult TopicPage(int topicId, bool showAddComment)
        {
            Dictionary<int, User> users = (Dictionary<int, User>)HttpContext.Application["users"];
            Dictionary<int, Topic> topics = (Dictionary<int, Topic>)HttpContext.Application["topics"];
            Dictionary<int, Comment> comments = (Dictionary<int, Comment>)HttpContext.Application["comments"];

            int current_user_id = (int)HttpContext.Session["current_user_id"];
            ViewBag.CurrentUser = users.ElementAt(current_user_id).Value;

            ViewBag.Topic = topics.ElementAt(topicId).Value;

            if (users[current_user_id].UpvotedTopics.Contains(topicId))
            {
                ViewBag.VoteType = "upvoted";
            }
            else if (users[current_user_id].DownvotedTopics.Contains(topicId))
            {
                ViewBag.VoteType = "downvoted";
            }
            else
            {
                ViewBag.VoteType = "nothing";
            }

            List<Comment> topicComments = new List<Comment>();
            List<Comment> userComments = new List<Comment>();

            foreach (Comment c in comments.Values)
            {
                if (c.TopicId == topicId)
                {
                    if (c.UserId == current_user_id)
                    {
                        userComments.Add(c);
                    }
                    else
                    {
                        topicComments.Add(c);
                    }
                }
            }

            ViewBag.TopicComments = topicComments;
            ViewBag.UserComments = userComments;
            ViewBag.Users = users.Values;

            if (showAddComment == true)
            {
                ViewBag.AddComment = true;
            }
            else
            {
                ViewBag.AddComment = false;
            }

            return View("TopicPage");
        }

        public ActionResult Upvote(int topicId)
        {
            Dictionary<int, User> users = (Dictionary<int, User>)HttpContext.Application["users"];
            Dictionary<int, Topic> topics = (Dictionary<int, Topic>)HttpContext.Application["topics"];
            int current_user_id = (int)HttpContext.Session["current_user_id"];

            if (users.ElementAt(current_user_id).Value.UpvotedTopics.Contains(topicId))
            {
                users.ElementAt(current_user_id).Value.UpvotedTopics.Remove(topicId);
                topics.ElementAt(topicId).Value.Upvote_number--;
                return RedirectToAction("TopicPage", "Forum", new { topicId = topicId, showAddComment = false });
            }

            if (users.ElementAt(current_user_id).Value.DownvotedTopics.Contains(topicId))
            {
                users.ElementAt(current_user_id).Value.DownvotedTopics.Remove(topicId);
                topics.ElementAt(topicId).Value.Downvote_number--;
            }

            users.ElementAt(current_user_id).Value.UpvotedTopics.Add(topicId);
            topics.ElementAt(topicId).Value.Upvote_number++;

            return RedirectToAction("TopicPage", "Forum", new { topicId = topicId, showAddComment = false });
        }

        public ActionResult Downvote(int topicId)
        {
            Dictionary<int, User> users = (Dictionary<int, User>)HttpContext.Application["users"];
            Dictionary<int, Topic> topics = (Dictionary<int, Topic>)HttpContext.Application["topics"];
            int current_user_id = (int)HttpContext.Session["current_user_id"];

            if (users.ElementAt(current_user_id).Value.DownvotedTopics.Contains(topicId))
            {
                users.ElementAt(current_user_id).Value.DownvotedTopics.Remove(topicId);
                topics.ElementAt(topicId).Value.Downvote_number--;
                return RedirectToAction("TopicPage", "Forum", new { topicId = topicId });
            }

            if (users.ElementAt(current_user_id).Value.UpvotedTopics.Contains(topicId))
            {
                users.ElementAt(current_user_id).Value.UpvotedTopics.Remove(topicId);
                topics.ElementAt(topicId).Value.Upvote_number--;
            }

            users.ElementAt(current_user_id).Value.DownvotedTopics.Add(topicId);
            topics.ElementAt(topicId).Value.Downvote_number++;

            return RedirectToAction("TopicPage", "Forum", new { topicId = topicId, showAddComment = false });
        }

        public ActionResult ShowAddComment(int topicId)
        {
            ViewBag.AddComment = true;
            return RedirectToAction("TopicPage", "Forum", new { topicId = topicId, showAddComment = false });
        }

        public ActionResult AddComment(int topicId, string text)
        {
            Dictionary<int, Comment> comments = (Dictionary<int, Comment>)HttpContext.Application["comments"];
            Dictionary<int, Topic> topics = (Dictionary<int, Topic>)HttpContext.Application["topics"];


            int current_user_id = (int)HttpContext.Session["current_user_id"];

            int commentId = comments.Count;

            comments.Add(commentId, new Comment(commentId, text, topicId, current_user_id));
            topics[topicId].Comment_number++;

            return RedirectToAction("TopicPage", "Forum", new { topicId = topicId, showAddComment = false });
        }

        public ActionResult DeleteComment(int commentId, int topicId)
        {
            Dictionary<int, Topic> topics = (Dictionary<int, Topic>)HttpContext.Application["topics"];
            Dictionary<int, Comment> comments = (Dictionary<int, Comment>)HttpContext.Application["comments"];

            topics[comments[commentId].TopicId].Comment_number--;
            comments.Remove(commentId);

            return RedirectToAction("TopicPage", "Forum", new { topicId = topicId, showAddComment = false });
        }
    }
}
