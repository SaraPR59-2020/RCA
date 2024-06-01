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
        UserDataRepository userDataRepository = new UserDataRepository();
        TopicDataRepository topicDataRepository = new TopicDataRepository();
        CommentDataRepositorycs commentDataRepository = new CommentDataRepositorycs();

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
            int current_user_id = (int)HttpContext.Session["current_user_id"];
            ViewBag.CurrentUser = userDataRepository.GetUser(current_user_id.ToString());

            ViewBag.Topic = topicDataRepository.GetTopic(topicId.ToString());

            if (userDataRepository.GetUser(current_user_id.ToString()).UpvotedTopics.Contains(topicId))
            {
                ViewBag.VoteType = "upvoted";
            }
            else if (userDataRepository.GetUser(current_user_id.ToString()).DownvotedTopics.Contains(topicId))
            {
                ViewBag.VoteType = "downvoted";
            }
            else
            {
                ViewBag.VoteType = "nothing";
            }

            List<Comment> topicComments = new List<Comment>();
            List<Comment> userComments = new List<Comment>();

            foreach (Comment c in commentDataRepository.RetrieveAllCommentss())
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
            ViewBag.Users = userDataRepository.RetrieveAllUsers();

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
            int current_user_id = (int)HttpContext.Session["current_user_id"];
            Topic topic = topicDataRepository.GetTopic(topicId.ToString());
            User user = userDataRepository.GetUser(current_user_id.ToString());

            if (user.UpvotedTopics.Contains(topicId))
            {
                user.UpvotedTopics.Remove(topicId);
                topic.Upvote_number--;
                userDataRepository.UpdateUser(user);
                topicDataRepository.UpdateTopic(topic);
                return RedirectToAction("TopicPage", "Forum", new { topicId = topicId, showAddComment = false });
            }

            if (user.DownvotedTopics.Contains(topicId))
            {
                user.DownvotedTopics.Remove(topicId);
                topic.Downvote_number--;
            }

            user.UpvotedTopics.Add(topicId);
            topic.Upvote_number++;
            userDataRepository.UpdateUser(user);
            topicDataRepository.UpdateTopic(topic);

            return RedirectToAction("TopicPage", "Forum", new { topicId = topicId, showAddComment = false });
        }

        public ActionResult Downvote(int topicId)
        {
            int current_user_id = (int)HttpContext.Session["current_user_id"];
            Topic topic = topicDataRepository.GetTopic(topicId.ToString());
            User user = userDataRepository.GetUser(current_user_id.ToString());

            if (user.DownvotedTopics.Contains(topicId))
            {
                user.DownvotedTopics.Remove(topicId);
                topic.Downvote_number--;
                userDataRepository.UpdateUser(user);
                topicDataRepository.UpdateTopic(topic);
                return RedirectToAction("TopicPage", "Forum", new { topicId = topicId });
            }

            if (user.UpvotedTopics.Contains(topicId))
            {
                user.UpvotedTopics.Remove(topicId);
                topic.Upvote_number--;
            }

            user.DownvotedTopics.Add(topicId);
            topic.Downvote_number++;
            userDataRepository.UpdateUser(user);
            topicDataRepository.UpdateTopic(topic);

            return RedirectToAction("TopicPage", "Forum", new { topicId = topicId, showAddComment = false });
        }

        public ActionResult ShowAddComment(int topicId)
        {
            ViewBag.AddComment = true;
            return RedirectToAction("TopicPage", "Forum", new { topicId = topicId, showAddComment = false });
        }

        public ActionResult AddComment(int topicId, string text)
        {
            int current_user_id = (int)HttpContext.Session["current_user_id"];

            int commentId = commentDataRepository.RetrieveAllCommentss().ToList().Count;

            Comment newComment = new Comment(commentId.ToString()) { Id = commentId, Text = text, TopicId = topicId, UserId = current_user_id };
            commentDataRepository.AddComment(newComment);
            Topic topic = topicDataRepository.GetTopic(topicId.ToString());
            topic.Comment_number++;
            topicDataRepository.UpdateTopic(topic);

            return RedirectToAction("TopicPage", "Forum", new { topicId = topicId, showAddComment = false });
        }

        public ActionResult DeleteComment(int commentId, int topicId)
        {
            topicDataRepository.GetTopic(commentDataRepository.GetComment(commentId.ToString()).TopicId.ToString()).Comment_number--;
            commentDataRepository.RemoveComment(commentId.ToString());
            Topic topic = topicDataRepository.GetTopic(topicId.ToString());
            topic.Comment_number--;
            topicDataRepository.UpdateTopic(topic);

            return RedirectToAction("TopicPage", "Forum", new { topicId = topicId, showAddComment = false });
        }
    }
}
