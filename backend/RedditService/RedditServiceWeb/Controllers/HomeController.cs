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
        UpvoteDataRepository upvoteDataRepository = new UpvoteDataRepository();
        DownvoteDataRepository downvoteDataRepository = new DownvoteDataRepository();
        SubscriptionDataRepository subscriptionDataRepository = new SubscriptionDataRepository();

        public ActionResult Index()
        {
            int current_user_id = (int)HttpContext.Session["current_user_id"];
            User current_user = userDataRepository.GetUser(current_user_id.ToString());
            List<Topic> userTopics = new List<Topic>();

            foreach (Topic t in topicDataRepository.RetrieveAllTopics())
            {
                if (t.User == current_user_id)
                {
                    userTopics.Add(t);
                }
            }
            //one kojima on pripada uzeti iz storage
            ViewBag.userTopics = userTopics;
            //ViewBag.id_prijavljenog = id_prijavljenog;
            ViewBag.User_name = current_user.Name;
            ViewBag.UserPicture = current_user.Image;
            // upvoted topics
            ViewBag.UpvotedTopics = upvoteDataRepository.GetTopicByUser(current_user_id);
            // downvoted topics
            ViewBag.DownvotedTopics = downvoteDataRepository.GetTopicByUser(current_user_id);
            // subscribed topics
            ViewBag.SubscribedTopics = subscriptionDataRepository.GetTopicByUser(current_user_id);

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

            foreach (Topic t in topicDataRepository.RetrieveAllTopics())
            {
                if (t.User == current_user_id)
                {
                    userTopics.Add(t);
                }
            }
            //one kojima on pripada uzeti iz storage
            ViewBag.userTopics = userTopics;
            //ViewBag.id_prijavljenog = id_prijavljenog;
            ViewBag.User_name = current_user.Name;
            ViewBag.UserPicture = current_user.Image;
            // upvoted topics
            ViewBag.UpvotedTopics = upvoteDataRepository.GetTopicByUser(current_user_id);
            // downvoted topics
            ViewBag.DownvotedTopics = downvoteDataRepository.GetTopicByUser(current_user_id);
            // subscribed topics
            ViewBag.SubscribedTopics = subscriptionDataRepository.GetTopicByUser(current_user_id);

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

            foreach (Topic t in topicDataRepository.RetrieveAllTopics())
            {
                if (t.User == current_user_id)
                {
                    userTopics.Add(t);
                }
            }
            //one kojima on pripada uzeti iz storage
            ViewBag.userTopics = userTopics;
            //ViewBag.id_prijavljenog = id_prijavljenog;
            ViewBag.User_name = current_user.Name;
            ViewBag.UserPicture = current_user.Image;
            // upvoted topics
            ViewBag.UpvotedTopics = upvoteDataRepository.GetTopicByUser(current_user_id);
            // downvoted topics
            ViewBag.DownvotedTopics = downvoteDataRepository.GetTopicByUser(current_user_id);
            // subscribed topics
            ViewBag.SubscribedTopics = subscriptionDataRepository.GetTopicByUser(current_user_id);

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

            //ViewBag.id_prijavljenog = id_prijavljenog;
            ViewBag.User_name = current_user.Name;
            ViewBag.UserPicture = current_user.Image;
            // upvoted topics
            ViewBag.UpvotedTopics = upvoteDataRepository.GetTopicByUser(current_user_id);
            // downvoted topics
            ViewBag.DownvotedTopics = downvoteDataRepository.GetTopicByUser(current_user_id);
            // subscribed topics
            ViewBag.SubscribedTopics = subscriptionDataRepository.GetTopicByUser(current_user_id);

            return View("Index");
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
                return RedirectToAction("Index", "Home");
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

            return RedirectToAction("Index", "Home");
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
                return RedirectToAction("Index", "Home");
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

            return RedirectToAction("Index", "Home");
        }

        public ActionResult DeleteTopic(int topicId)
        {
            int current_user_id = (int)HttpContext.Session["current_user_id"];
            IQueryable<Downvote> downvotes = downvoteDataRepository.RetrieveAllDownvotes();
            IQueryable<Upvote> upvotes = upvoteDataRepository.RetrieveAllUpvotes();

            if (upvoteDataRepository.GetTopicByUser(current_user_id).Contains(topicId))
            {
                upvoteDataRepository.RemoveUpvote(upvotes.Where(t => t.Topic_id == topicId).FirstOrDefault().Upvote_id.ToString());
            }
            if (downvoteDataRepository.GetTopicByUser(current_user_id).Contains(topicId))
            {
                downvoteDataRepository.RemoveDownvote(downvotes.Where(t => t.Topic_id == topicId).FirstOrDefault().Downvote_id.ToString());
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

            int number_of_subscriptions = subscriptionDataRepository.RetrieveAllSubscriptions().ToList().Count();
            Subscription newSubscription = new Subscription(number_of_subscriptions.ToString()) { Subscription_id = number_of_subscriptions, User_id = current_user_id, Topic_id = topicId };

            subscriptionDataRepository.AddSubscription(newSubscription);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Unsubscribe(int topicId)
        {
            subscriptionDataRepository.RemoveSubscription(subscriptionDataRepository.RetrieveAllSubscriptions().Where(t => t.Topic_id == topicId).FirstOrDefault().Subscription_id.ToString());

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