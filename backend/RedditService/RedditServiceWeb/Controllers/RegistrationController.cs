using RedditServiceWeb.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedditServiceWeb.Controllers
{
    public class RegistrationController : Controller
    {
        // GET: Registration
        public ActionResult RegistrationPage()
        {
            return View("RegistrationPage");
        }

        [HttpPost]
        public ActionResult Registration(string Name, string Surname, string Address, string City, string Country, string Phone_number, string Email, string Password, HttpPostedFileBase Image)
        {
            Dictionary<int, User> users = (Dictionary<int, User>)HttpContext.Application["users"];
            int number_of_users = users.Count;
            string fileName = "";
            string path = "";
            try
            {
                if (Image.ContentLength > 0)
                {

                    fileName = Path.GetFileName(Image.FileName);
                    path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                    Image.SaveAs(path);
                }
                User newUser = new User(number_of_users + 1, Name, Surname, Address, City, Country, Phone_number, Email, Password, $"/Images/{fileName}");
                users.Add(number_of_users + 1, newUser);
                HttpContext.Application["users"] = users;
                Trace.WriteLine("Registration successfull.");
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
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            return RedirectToAction("RegistrationPage", "Registration");

        }
    }
}