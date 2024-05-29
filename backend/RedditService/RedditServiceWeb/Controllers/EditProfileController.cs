using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedditServiceWeb.Controllers
{
    public class EditProfileController : Controller
    {
        // GET: EditProfile
        public ActionResult EditProfilePage()
        {
            Dictionary<int, User> users = (Dictionary<int, User>)HttpContext.Application["users"];
            int current_user_id = (int)HttpContext.Session["current_user_id"];
            User current_user = users[current_user_id];
            ViewBag.User = current_user;
            return View("EditProfilePage");
        }
        public ActionResult EditProfile(string Name, string Surname, string Address, string City, string Country, string Phone_number, string Email, string Password, HttpPostedFileBase Image)
        {
            Dictionary<int, User> users = (Dictionary<int, User>)HttpContext.Application["users"];
            int current_user_id = (int)HttpContext.Session["current_user_id"];
            string fileName = "";
            string path = "";
            if (Image != null)
            {
                if (Image.ContentLength > 0)
                {

                    fileName = Path.GetFileName(Image.FileName);
                    path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                    Image.SaveAs(path);
                    users[current_user_id].Image = $"/Images/{fileName}";
                }
            }

            users[current_user_id].Name = Name;
            users[current_user_id].Surname = Surname;
            users[current_user_id].Address = Address;
            users[current_user_id].City = City;
            users[current_user_id].Country = Country;
            users[current_user_id].Phone_number = Phone_number;
            users[current_user_id].Email = Email;
            users[current_user_id].Password = Password;

            return RedirectToAction("Index", "Home");
        }
    }
}