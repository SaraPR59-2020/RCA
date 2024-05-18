using System.Web.Mvc;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

using RedditServiceWeb.Models;

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
    }
}
