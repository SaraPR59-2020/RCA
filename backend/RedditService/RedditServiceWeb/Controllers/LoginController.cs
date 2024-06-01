using Common;
using RedditServiceWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RedditServiceWeb.Controllers
{
    public class LoginController : Controller
    {
        UserDataRepository userDataRepository = new UserDataRepository();
        // GET: Login
        public ActionResult LoginPage()
        {
            return View("LoginPage");
        }

        public ActionResult Login(string Email, string Password)
        {
            ViewBag.Error = false;
            if (HttpContext.Session["current_user_id"] == null || (int)HttpContext.Session["current_user_id"] == -1)
            {
                foreach (User u in userDataRepository.RetrieveAllUsers())
                {
                    if (u.Email == Email && u.Password == GenerateHash(Password))
                    {
                        HttpContext.Session["current_user_id"] = Int32.Parse(u.RowKey);
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            ViewBag.Error = true;
            return View("LoginPage");
        }

        public ActionResult Logoff()
        {
            HttpContext.Session["current_user_id"] = -1;
            return RedirectToAction("LoginPage", "Login");
        }

        private string GenerateHash(string toHash)
        {
            var crypt = new SHA256Managed();
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(toHash));
            foreach (byte theByte in crypto)
            {
                hash += theByte.ToString("x2");
            }
            return hash;
        }
    }
}