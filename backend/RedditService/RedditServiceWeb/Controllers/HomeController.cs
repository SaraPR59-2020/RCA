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
            
            foreach (Topic t in topics.Values)
            {
                if (t.User == current_user_id)
                {
                    userTopics.Add(t);
                }
            }
            ViewBag.userTopics = userTopics;
            //ViewBag.id_prijavljenog = id_prijavljenog;
            ViewBag.User_name = current_user.Name;
            ViewBag.topics = topics.Values;
            return View();
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