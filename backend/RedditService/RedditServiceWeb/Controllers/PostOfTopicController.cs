using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace RedditServiceWeb.Controllers
{
    public class PostOfTopicController : Controller
    {
        // GET: PostOfTopic
        public ActionResult PostOftopic()
        {
            return View("PostOftopic");
        }
    }
}