using Common;
using ImageConverterWorker;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
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

            if (Image != null)
            {
                string uniqueBlobName = string.Format("image_{0}", current_user_id);
                var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
                CloudBlobClient blobStorage = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobStorage.GetContainerReference("reddit");
                CloudBlockBlob blob = container.GetBlockBlobReference(uniqueBlobName);
                blob.Properties.ContentType = Image.ContentType;
                // postavljanje odabrane datoteke (slike) u blob servis koristeci blob klijent
                blob.UploadFromStream(Image.InputStream);

                current_user.Name = Name;
                current_user.Surname = Surname;
                current_user.Address = Address;
                current_user.City = City;
                current_user.Country = Country;
                current_user.Phone_number = Phone_number;
                current_user.Email = Email;
                current_user.Password = GenerateHash(Password);
                current_user.PhotoUrl = blob.Uri.ToString();
                current_user.ThumbnailUrl = blob.Uri.ToString();

                userDataRepository.UpdateUser(current_user);

                CloudQueue queue = QueueHelper.GetQueueReference("reddit");
                string message = current_user_id.ToString() + "_User";
                queue.AddMessage(new CloudQueueMessage(message), null, TimeSpan.FromMilliseconds(30));
            }

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