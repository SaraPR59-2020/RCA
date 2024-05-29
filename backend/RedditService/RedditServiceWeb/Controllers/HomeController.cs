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
        public ActionResult Index()
        {
            Dictionary<int, User> users = (Dictionary<int, User>)HttpContext.Application["users"];
            Dictionary<int, Topic> topics = (Dictionary<int, Topic>)HttpContext.Application["topics"];

            int current_user_id = (int)HttpContext.Session["current_user_id"];
            User current_user = users[current_user_id];
            List<Topic> userTopics = new List<Topic>();
            List<int> upvotedTopics = new List<int>();
            List<int> downvotedTopics = new List<int>();
            List<int> subscribedTopics = new List<int>();

            foreach (Topic t in topics.Values)
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
            ViewBag.topics = topics.Values;
            if (ViewBag.ShowTopics == null)
            {
                ViewBag.ShowTopics = "Your topics";
            }
            return View();
        }

        [HttpPost]
        public ActionResult Search(string topicPage, string headline)
        {
            Dictionary<int, User> users = (Dictionary<int, User>)HttpContext.Application["users"];
            Dictionary<int, Topic> topics = (Dictionary<int, Topic>)HttpContext.Application["topics"];
            List<Topic> searchTopics = new List<Topic>();

            int current_user_id = (int)HttpContext.Session["current_user_id"];
            User current_user = users[current_user_id];
            List<Topic> userTopics = new List<Topic>();
            List<int> upvotedTopics = new List<int>();
            List<int> downvotedTopics = new List<int>();
            List<int> subscribedTopics = new List<int>();

            foreach (Topic t in topics.Values)
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
                foreach (Topic t in topics.Values)
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
                foreach (Topic t in topics.Values)
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
            Dictionary<int, User> users = (Dictionary<int, User>)HttpContext.Application["users"];
            Dictionary<int, Topic> topics = (Dictionary<int, Topic>)HttpContext.Application["topics"];
            List<Topic> sortTopics = new List<Topic>();

            int current_user_id = (int)HttpContext.Session["current_user_id"];
            User current_user = users[current_user_id];
            List<Topic> userTopics = new List<Topic>();
            List<int> upvotedTopics = new List<int>();
            List<int> downvotedTopics = new List<int>();
            List<int> subscribedTopics = new List<int>();

            foreach (Topic t in topics.Values)
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
                foreach (Topic t in topics.Values)
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
                    ViewBag.topics = topics.Values.OrderBy(o => o.Headline).ToList();
                    ViewBag.ShowTopics = "All topics";
                    return View("Index");
                }
                else if (sortType == "Z-A")
                {
                    ViewBag.topics = topics.Values.OrderByDescending(o => o.Headline).ToList();
                    ViewBag.ShowTopics = "All topics";
                    return View("Index");
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult AllTopics()
        {
            Dictionary<int, User> users = (Dictionary<int, User>)HttpContext.Application["users"];
            Dictionary<int, Topic> topics = (Dictionary<int, Topic>)HttpContext.Application["topics"];
            int current_user_id = (int)HttpContext.Session["current_user_id"];
            User current_user = users[current_user_id];
            ViewBag.ShowTopics = "All topics";
            ViewBag.topics = topics.Values;

            List<int> upvotedTopics = new List<int>();
            List<int> downvotedTopics = new List<int>();
            List<int> subscribedTopics = new List<int>();

            foreach (Topic t in topics.Values)
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
            Dictionary<int, User> users = (Dictionary<int, User>)HttpContext.Application["users"];
            Dictionary<int, Topic> topics = (Dictionary<int, Topic>)HttpContext.Application["topics"];
            int current_user_id = (int)HttpContext.Session["current_user_id"];

            if (users.ElementAt(current_user_id).Value.UpvotedTopics.Contains(topicId))
            {
                users.ElementAt(current_user_id).Value.UpvotedTopics.Remove(topicId);
                topics.ElementAt(topicId).Value.Upvote_number--;
                return RedirectToAction("Index", "Home");
            }

            if (users.ElementAt(current_user_id).Value.DownvotedTopics.Contains(topicId))
            {
                users.ElementAt(current_user_id).Value.DownvotedTopics.Remove(topicId);
                topics.ElementAt(topicId).Value.Downvote_number--;
            }

            users.ElementAt(current_user_id).Value.UpvotedTopics.Add(topicId);
            topics.ElementAt(topicId).Value.Upvote_number++;

            return RedirectToAction("Index", "Home");
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
                return RedirectToAction("Index", "Home");
            }

            if (users.ElementAt(current_user_id).Value.UpvotedTopics.Contains(topicId))
            {
                users.ElementAt(current_user_id).Value.UpvotedTopics.Remove(topicId);
                topics.ElementAt(topicId).Value.Upvote_number--;
            }

            users.ElementAt(current_user_id).Value.DownvotedTopics.Add(topicId);
            topics.ElementAt(topicId).Value.Downvote_number++;

            return RedirectToAction("Index", "Home");
        }

        public ActionResult DeleteTopic(int topicId)
        {
            Dictionary<int, User> users = (Dictionary<int, User>)HttpContext.Application["users"];
            Dictionary<int, Topic> topics = (Dictionary<int, Topic>)HttpContext.Application["topics"];
            Dictionary<int, Comment> comments = (Dictionary<int, Comment>)HttpContext.Application["comments"];
            int current_user_id = (int)HttpContext.Session["current_user_id"];

            foreach (User u in users.Values)
            {
                if (u.UpvotedTopics.Contains(topicId))
                {
                    u.UpvotedTopics.Remove(topicId);
                }
                if (u.DownvotedTopics.Contains(topicId))
                {
                    u.DownvotedTopics.Remove(topicId);
                }
            }

            List<int> commentsToDelete = new List<int>();
            foreach (Comment c in comments.Values)
            {
                if (c.TopicId == topicId)
                {
                    commentsToDelete.Add(c.Id);
                }
            }

            foreach (int c in commentsToDelete)
            {
                comments.Remove(c);
            }

            topics.Remove(topicId);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Subscribe(int topicId)
        {
            Dictionary<int, User> users = (Dictionary<int, User>)HttpContext.Application["users"];
            int current_user_id = (int)HttpContext.Session["current_user_id"];

            users[current_user_id].SubscribedTopics.Add(topicId);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Unsubscribe(int topicId)
        {
            Dictionary<int, User> users = (Dictionary<int, User>)HttpContext.Application["users"];
            int current_user_id = (int)HttpContext.Session["current_user_id"];

            users[current_user_id].SubscribedTopics.Remove(topicId);

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