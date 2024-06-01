using Common;
using RedditServiceWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedditServiceWeb.Controllers
{
    public class HomeController : Controller
    {
        UserDataRepository userDataRepository = new UserDataRepository();
        TopicDataRepository topicDataRepository = new TopicDataRepository();
        CommentDataRepositorycs commentDataRepository = new CommentDataRepositorycs();
        public ActionResult Index()
        {
            int current_user_id = (int)HttpContext.Session["current_user_id"];
            User current_user = userDataRepository.GetUser(current_user_id.ToString());
            List<Topic> userTopics = new List<Topic>();
            List<int> upvotedTopics = new List<int>();
            List<int> downvotedTopics = new List<int>();
            List<int> subscribedTopics = new List<int>();

            foreach (Topic t in topicDataRepository.RetrieveAllTopics())
            {
                if (t.User == current_user_id)
                {
                    userTopics.Add(t);
                }
                if (current_user.UpvotedTopics.Contains(t.Topic_id))
                {
                    upvotedTopics.Add(t.Topic_id);
                }
                if (current_user.DownvotedTopics.Contains(t.Topic_id))
                {
                    downvotedTopics.Add(t.Topic_id);
                }
                if (current_user.SubscribedTopics.Contains(t.Topic_id))
                {
                    subscribedTopics.Add(t.Topic_id);
                }
            }
            //one kojima on pripada uzeti iz storage
            ViewBag.userTopics = userTopics;
            //ViewBag.id_prijavljenog = id_prijavljenog;
            ViewBag.User_name = current_user.Name;
            ViewBag.UserPicture = current_user.Image;
            // upvoted topics
            ViewBag.UpvotedTopics = upvotedTopics;
            // downvoted topics
            ViewBag.DownvotedTopics = downvotedTopics;
            // subscribed topics
            ViewBag.SubscribedTopics = subscribedTopics;

            //uzeti sve iz storagea 
            ViewBag.topics = topicDataRepository.RetrieveAllTopics();
            if (ViewBag.ShowTopics == null)
            {
                ViewBag.ShowTopics = "Your topics";
            }
            return View();
        }

        [HttpPost]
        public ActionResult Search(string topicPage, string headline)
        {
            List<Topic> searchTopics = new List<Topic>();

            int current_user_id = (int)HttpContext.Session["current_user_id"];
            User current_user = userDataRepository.RetrieveAllUsers().Where(u => u.User_id == current_user_id).First();
            List<Topic> userTopics = new List<Topic>();
            List<int> upvotedTopics = new List<int>();
            List<int> downvotedTopics = new List<int>();
            List<int> subscribedTopics = new List<int>();

            foreach (Topic t in topicDataRepository.RetrieveAllTopics())
            {
                if (t.User == current_user_id)
                {
                    userTopics.Add(t);
                }
                if (current_user.UpvotedTopics.Contains(t.Topic_id))
                {
                    upvotedTopics.Add(t.Topic_id);
                }
                if (current_user.DownvotedTopics.Contains(t.Topic_id))
                {
                    downvotedTopics.Add(t.Topic_id);
                }
                if (current_user.SubscribedTopics.Contains(t.Topic_id))
                {
                    subscribedTopics.Add(t.Topic_id);
                }
            }
            //one kojima on pripada uzeti iz storage
            ViewBag.userTopics = userTopics;
            //ViewBag.id_prijavljenog = id_prijavljenog;
            ViewBag.User_name = current_user.Name;
            ViewBag.UserPicture = current_user.Image;
            // upvoted topics
            ViewBag.UpvotedTopics = upvotedTopics;
            // downvoted topics
            ViewBag.DownvotedTopics = downvotedTopics;
            // subscribed topics
            ViewBag.SubscribedTopics = subscribedTopics;

            if (topicPage == "Your topics")
            {
                foreach (Topic t in topicDataRepository.RetrieveAllTopics())
                {
                    if (t.Headline.Contains(headline) && t.User == current_user_id)
                    {
                        searchTopics.Add(t);
                    }
                }
                ViewBag.userTopics = searchTopics;
                ViewBag.ShowTopics = "Your topics";
                return View("Index");
            }
            else if (topicPage == "All topics")
            {
                foreach (Topic t in topicDataRepository.RetrieveAllTopics())
                {
                    if (t.Headline.Contains(headline))
                    {
                        searchTopics.Add(t);
                    }
                }
                ViewBag.topics = searchTopics;
                ViewBag.ShowTopics = "All topics";
                return View("Index");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Sort(string topicPage, string sortType)
        {
            List<Topic> sortTopics = new List<Topic>();

            int current_user_id = (int)HttpContext.Session["current_user_id"];
            User current_user = userDataRepository.GetUser(current_user_id.ToString());
            List<Topic> userTopics = new List<Topic>();
            List<int> upvotedTopics = new List<int>();
            List<int> downvotedTopics = new List<int>();
            List<int> subscribedTopics = new List<int>();

            foreach (Topic t in topicDataRepository.RetrieveAllTopics())
            {
                if (t.User == current_user_id)
                {
                    userTopics.Add(t);
                }
                if (current_user.UpvotedTopics.Contains(t.Topic_id))
                {
                    upvotedTopics.Add(t.Topic_id);
                }
                if (current_user.DownvotedTopics.Contains(t.Topic_id))
                {
                    downvotedTopics.Add(t.Topic_id);
                }
                if (current_user.SubscribedTopics.Contains(t.Topic_id))
                {
                    subscribedTopics.Add(t.Topic_id);
                }
            }
            //one kojima on pripada uzeti iz storage
            ViewBag.userTopics = userTopics;
            //ViewBag.id_prijavljenog = id_prijavljenog;
            ViewBag.User_name = current_user.Name;
            ViewBag.UserPicture = current_user.Image;
            // upvoted topics
            ViewBag.UpvotedTopics = upvotedTopics;
            // downvoted topics
            ViewBag.DownvotedTopics = downvotedTopics;
            // subscribed topics
            ViewBag.SubscribedTopics = subscribedTopics;

            if (topicPage == "Your topics")
            {
                foreach (Topic t in topicDataRepository.RetrieveAllTopics())
                {
                    if (t.User == current_user_id)
                    {
                        sortTopics.Add(t);
                    }
                }
                if (sortType == "A-Z")
                {
                    ViewBag.userTopics = sortTopics.OrderBy(o => o.Headline).ToList();
                    ViewBag.ShowTopics = "Your topics";
                    return View("Index");
                }
                else if (sortType == "Z-A")
                {
                    ViewBag.userTopics = sortTopics.OrderByDescending(o => o.Headline).ToList();
                    ViewBag.ShowTopics = "Your topics";
                    return View("Index");
                }
            }
            else if (topicPage == "All topics")
            {
                if (sortType == "A-Z")
                {
                    ViewBag.topics = topicDataRepository.RetrieveAllTopics().OrderBy(o => o.Headline).ToList();
                    ViewBag.ShowTopics = "All topics";
                    return View("Index");
                }
                else if (sortType == "Z-A")
                {
                    ViewBag.topics = topicDataRepository.RetrieveAllTopics().OrderByDescending(o => o.Headline).ToList();
                    ViewBag.ShowTopics = "All topics";
                    return View("Index");
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult AllTopics()
        {
            int current_user_id = (int)HttpContext.Session["current_user_id"];
            User current_user = userDataRepository.RetrieveAllUsers().Where(u => u.User_id == current_user_id).First();
            ViewBag.ShowTopics = "All topics";
            ViewBag.topics = topicDataRepository.RetrieveAllTopics();

            List<int> upvotedTopics = new List<int>();
            List<int> downvotedTopics = new List<int>();
            List<int> subscribedTopics = new List<int>();

            foreach (Topic t in topicDataRepository.RetrieveAllTopics())
            {
                if (current_user.UpvotedTopics.Contains(t.Topic_id))
                {
                    upvotedTopics.Add(t.Topic_id);
                }
                if (current_user.DownvotedTopics.Contains(t.Topic_id))
                {
                    downvotedTopics.Add(t.Topic_id);
                }
                if (current_user.SubscribedTopics.Contains(t.Topic_id))
                {
                    subscribedTopics.Add(t.Topic_id);
                }
            }

            //ViewBag.id_prijavljenog = id_prijavljenog;
            ViewBag.User_name = current_user.Name;
            ViewBag.UserPicture = current_user.Image;
            // upvoted topics
            ViewBag.UpvotedTopics = upvotedTopics;
            // downvoted topics
            ViewBag.DownvotedTopics = downvotedTopics;
            // subscribed topics
            ViewBag.SubscribedTopics = subscribedTopics;

            return View("Index");
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
                return RedirectToAction("Index", "Home");
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

            return RedirectToAction("Index", "Home");
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
                return RedirectToAction("Index", "Home");
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

            return RedirectToAction("Index", "Home");
        }

        public ActionResult DeleteTopic(int topicId)
        {
            int current_user_id = (int)HttpContext.Session["current_user_id"];

            foreach (User u in userDataRepository.RetrieveAllUsers())
            {
                if (u.UpvotedTopics.Contains(topicId))
                {
                    u.UpvotedTopics.Remove(topicId);
                    userDataRepository.UpdateUser(u);
                }
                if (u.DownvotedTopics.Contains(topicId))
                {
                    u.DownvotedTopics.Remove(topicId);
                    userDataRepository.UpdateUser(u);
                }
            }

            List<int> commentsToDelete = new List<int>();
            foreach (Comment c in commentDataRepository.RetrieveAllCommentss())
            {
                if (c.TopicId == topicId)
                {
                    commentsToDelete.Add(c.Id);
                }
            }

            foreach (int c in commentsToDelete)
            {
                commentDataRepository.RemoveComment(c.ToString());
            }

            topicDataRepository.RemoveTopic(topicId.ToString());
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Subscribe(int topicId)
        {
            int current_user_id = (int)HttpContext.Session["current_user_id"];
            User user = userDataRepository.GetUser(current_user_id.ToString());

            user.SubscribedTopics.Add(topicId);
            userDataRepository.UpdateUser(user);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Unsubscribe(int topicId)
        {
            int current_user_id = (int)HttpContext.Session["current_user_id"];
            User user = userDataRepository.GetUser(current_user_id.ToString());

            user.SubscribedTopics.Remove(topicId);
            userDataRepository.UpdateUser(user);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}