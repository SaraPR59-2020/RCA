using Common;
using ImageConverterWorker;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
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
            try
            {
                string uniqueBlobName = string.Format("image_{0}", number_of_users);
                var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
                CloudBlobClient blobStorage = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobStorage.GetContainerReference("reddit");
                CloudBlockBlob blob = container.GetBlockBlobReference(uniqueBlobName);
                blob.Properties.ContentType = Image.ContentType;
                // postavljanje odabrane datoteke (slike) u blob servis koristeci blob klijent
                blob.UploadFromStream(Image.InputStream);

                User newUser = new User(number_of_users.ToString()) { User_id = number_of_users, Name = Name, Surname = Surname, Address = Address, City = City, Country = Country, Phone_number = Phone_number, Email = Email, Password = GenerateHash(Password), PhotoUrl = blob.Uri.ToString(), ThumbnailUrl = blob.Uri.ToString() };
                userDataRepository.AddUser(newUser);

                CloudQueue queue = QueueHelper.GetQueueReference("reddit");
                string message = number_of_users.ToString() + "_User";
                queue.AddMessage(new CloudQueueMessage(message), null, TimeSpan.FromMilliseconds(30));

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