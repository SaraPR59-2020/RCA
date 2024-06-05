using Common;
using ImageConverterWorker;
using Microsoft.Azure;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedditServiceWeb.Controllers
{
    public class NewTopicController : Controller
    {
        TopicDataRepository topicDataRepository = new TopicDataRepository();
        // GET: NewTopic
        public ActionResult AddNewTopicPage()
        {
            return View("AddNewTopicPage");
        }

        [HttpPost]
        public ActionResult AddNewTopic(string Headline, string Content, HttpPostedFileBase Image)
        {
            int current_user_id = (int)HttpContext.Session["current_user_id"];
            int new_topic_id = topicDataRepository.RetrieveAllTopics().ToList().Count;
            try
            {
                if (Image != null)
                {
                    string uniqueBlobName = string.Format("image_{0}", new_topic_id);
                    var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
                    CloudBlobClient blobStorage = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = blobStorage.GetContainerReference("reddit");
                    CloudBlockBlob blob = container.GetBlockBlobReference(uniqueBlobName);
                    blob.Properties.ContentType = Image.ContentType;
                    // postavljanje odabrane datoteke (slike) u blob servis koristeci blob klijent
                    blob.UploadFromStream(Image.InputStream);
                    
                    Topic newTopic = new Topic(new_topic_id.ToString()) { Topic_id = new_topic_id, Headline = Headline, Content = Content, User = current_user_id, Upvote_number = 0, Downvote_number = 0, Comment_number = 0, PhotoUrl = blob.Uri.ToString(), ThumbnailUrl = blob.Uri.ToString() };
                    topicDataRepository.AddTopic(newTopic);

                    CloudQueue queue = QueueHelper.GetQueueReference("reddit");
                    string message = new_topic_id.ToString() + "_Topic";
                    queue.AddMessage(new CloudQueueMessage(message), null, TimeSpan.FromMilliseconds(30));
                }

                Trace.WriteLine("New topic successfully added.");
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }

            return RedirectToAction("AddNewTopicPage", "NewTopic");

        }
    }
}