using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RedditServiceWeb.Controllers
{
    public class EditProfileController : Controller
    {
        UserDataRepository userDataRepository = new UserDataRepository();
        // GET: EditProfile
        public ActionResult EditProfilePage()
        {
            int current_user_id = (int)HttpContext.Session["current_user_id"];
            User current_user = userDataRepository.GetUser(current_user_id.ToString());
            ViewBag.User = current_user;
            return View("EditProfilePage");
        }
        public ActionResult EditProfile(string Name, string Surname, string Address, string City, string Country, string Phone_number, string Email, string Password, HttpPostedFileBase Image)
        {
            int current_user_id = (int)HttpContext.Session["current_user_id"];
            User current_user = userDataRepository.GetUser(current_user_id.ToString());
            string fileName = "";
            string path = "";
            if (Image != null)
            {
                if (Image.ContentLength > 0)
                {

                    fileName = Path.GetFileName(Image.FileName);
                    path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                    Image.SaveAs(path);
                    current_user.Image = $"/Images/{fileName}";
                }
            }

            current_user.Name = Name;
            current_user.Surname = Surname;
            current_user.Address = Address;
            current_user.City = City;
            current_user.Country = Country;
            current_user.Phone_number = Phone_number;
            current_user.Email = Email;
            current_user.Password = GenerateHash(Password);

            userDataRepository.UpdateUser(current_user);

            return RedirectToAction("Index", "Home");
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