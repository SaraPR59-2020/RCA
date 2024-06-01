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
        TopicDataRepository topicDataRepository = new TopicDataRepository();
        // GET: NewTopic
        public ActionResult AddNewTopicPage()
        {
            return View("AddNewTopicPage");
        }

        [HttpPost]
        public ActionResult AddNewTopic(string Headline, string Content, HttpPostedFileBase Image)
        {
            int current_user_id = (int)HttpContext.Session["current_user_id"];
            int new_topic_id = topicDataRepository.RetrieveAllTopics().ToList().Count;
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
                Topic newTopic = new Topic(new_topic_id.ToString()) { Topic_id = new_topic_id, Headline = Headline, Content = Content, User = current_user_id, Upvote_number = 0, Downvote_number = 0, Comment_number = 0, Topic_image = $"/Images/{fileName}" };
                topicDataRepository.AddTopic(newTopic);
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