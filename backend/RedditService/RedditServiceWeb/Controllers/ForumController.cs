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
        UpvoteDataRepository upvoteDataRepository = new UpvoteDataRepository();
        DownvoteDataRepository downvoteDataRepository = new DownvoteDataRepository();

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

            if (upvoteDataRepository.GetTopicByUser(current_user_id).Contains(topicId))
            {
                ViewBag.VoteType = "upvoted";
            }
            else if (downvoteDataRepository.GetTopicByUser(current_user_id).Contains(topicId))
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
            IQueryable<Downvote> downvotes = downvoteDataRepository.RetrieveAllDownvotes();
            IQueryable<Upvote> upvotes = upvoteDataRepository.RetrieveAllUpvotes();

            if (upvoteDataRepository.GetTopicByUser(current_user_id).Contains(topicId))
            {
                upvoteDataRepository.RemoveUpvote(upvotes.Where(t => t.Topic_id == topicId).FirstOrDefault().Upvote_id.ToString());
                topic.Upvote_number--;
                topicDataRepository.UpdateTopic(topic);
                return RedirectToAction("TopicPage", "Forum", new { topicId = topicId, showAddComment = false });
            }

            if (downvoteDataRepository.GetTopicByUser(current_user_id).Contains(topicId))
            {
                downvoteDataRepository.RemoveDownvote(downvotes.Where(t => t.Topic_id == topicId).FirstOrDefault().Downvote_id.ToString());
                topic.Downvote_number--;
            }

            int newUpvote_id = upvotes.ToList().Count();
            Upvote newUpvote = new Upvote(newUpvote_id.ToString()) { Upvote_id = newUpvote_id, User_id = current_user_id, Topic_id = topicId };

            upvoteDataRepository.AddUpvote(newUpvote);
            topic.Upvote_number++;
            topicDataRepository.UpdateTopic(topic);

            return RedirectToAction("TopicPage", "Forum", new { topicId = topicId, showAddComment = false });
        }

        public ActionResult Downvote(int topicId)
        {
            int current_user_id = (int)HttpContext.Session["current_user_id"];
            Topic topic = topicDataRepository.GetTopic(topicId.ToString());
            IQueryable<Downvote> downvotes = downvoteDataRepository.RetrieveAllDownvotes();
            IQueryable<Upvote> upvotes = upvoteDataRepository.RetrieveAllUpvotes();

            if (downvoteDataRepository.GetTopicByUser(current_user_id).Contains(topicId))
            {
                downvoteDataRepository.RemoveDownvote(downvotes.Where(t => t.Topic_id == topicId).FirstOrDefault().Downvote_id.ToString());
                topic.Downvote_number--;
                topicDataRepository.UpdateTopic(topic);
                return RedirectToAction("TopicPage", "Forum", new { topicId = topicId, showAddComment = false });
            }

            if (upvoteDataRepository.GetTopicByUser(current_user_id).Contains(topicId))
            {
                upvoteDataRepository.RemoveUpvote(upvotes.Where(t => t.Topic_id == topicId).FirstOrDefault().Upvote_id.ToString());
                topic.Upvote_number--;
            }

            int newDownvote_id = downvotes.ToList().Count();
            Downvote newDownvote = new Downvote(newDownvote_id.ToString()) { Downvote_id = newDownvote_id, User_id = current_user_id, Topic_id = topicId };

            downvoteDataRepository.AddDownvote(newDownvote);
            topic.Downvote_number++;
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
