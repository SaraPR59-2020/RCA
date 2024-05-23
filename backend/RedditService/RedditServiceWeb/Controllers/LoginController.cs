using RedditServiceWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedditServiceWeb.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult LoginPage()
        {
            return View("LoginPage");
        }

        public ActionResult Login(string Email, string Password)
        {
            Dictionary<int, User> users = (Dictionary<int, User>)HttpContext.Application["users"];
            //int id_prijavljenog = (int)HttpContext.Session["id_prijavljenog"];
            ViewBag.Error = false;
            if (HttpContext.Session["current_user_id"] == null || (int)HttpContext.Session["current_user_id"] == -1)
            {
                foreach (User u in users.Values)
                {
                    if (u.Email == Email && u.Password == Password)
                    {
                        HttpContext.Session["current_user_id"] = u.User_id;
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            ViewBag.Error = true;
            return View("LoginPage");
        }

        public ActionResult Logoff()
        {
            HttpContext.Session["id_prijavljenog"] = -1;
            return RedirectToAction("LoginPage", "Login");
        }
    }
}