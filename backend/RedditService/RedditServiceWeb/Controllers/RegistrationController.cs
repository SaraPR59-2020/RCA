using Common;
using RedditServiceWeb.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RedditServiceWeb.Controllers
{
    public class RegistrationController : Controller
    {
        UserDataRepository userDataRepository = new UserDataRepository();
        // GET: Registration
        public ActionResult RegistrationPage()
        {
            return View("RegistrationPage");
        }

        [HttpPost]
        public ActionResult Registration(string Name, string Surname, string Address, string City, string Country, string Phone_number, string Email, string Password, HttpPostedFileBase Image)
        {
            int number_of_users = userDataRepository.RetrieveAllUsers().ToList().Count;
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
                User newUser = new User(number_of_users.ToString()) { User_id = number_of_users, Name = Name, Surname = Surname, Address = Address, City = City, Country = Country, Phone_number = Phone_number, Email = Email, Password = GenerateHash(Password), Image = $"/Images/{fileName}" };
                userDataRepository.AddUser(newUser);
                Trace.WriteLine("Registration successfull.");
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
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            return RedirectToAction("RegistrationPage", "Registration");

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