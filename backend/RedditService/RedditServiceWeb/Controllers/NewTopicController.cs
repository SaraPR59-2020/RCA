using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedditServiceWeb.Controllers
{
    public class NewTopicController : Controller
    {
        // GET: NewTopic
        public ActionResult AddNewTopicPage()
        {
            return View("AddNewTopicPage");
        }

        [HttpPost]
        public ActionResult AddNewTopic(string Headline, string Content, HttpPostedFileBase Image)
        {
            Dictionary<int, Topic> topics = (Dictionary<int, Topic>)HttpContext.Application["topics"];
            int current_user_id = (int)HttpContext.Session["current_user_id"];
            int new_topic_id = topics.Count;
            string fileName = "";
            string path = "";
            try
            {
                if (Image != null)
                {

                    fileName = Path.GetFileName(Image.FileName);
                    path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                    Image.SaveAs(path);
                }
                Topic newTopic = new Topic(new_topic_id, Headline, Content, current_user_id, 0, 0, 0, $"/Images/{fileName}");
                topics.Add(new_topic_id, newTopic);
                HttpContext.Application["topics"] = topics;
                Trace.WriteLine("New topic successfully added.");
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }

            return RedirectToAction("AddNewTopicPage", "NewTopic");

        }
    }
}