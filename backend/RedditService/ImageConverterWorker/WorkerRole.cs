using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Common;
using System.Drawing;

namespace ImageConverterWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            CloudQueue queue = QueueHelper.GetQueueReference("reddit");
            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("ImageConverter_WorkerRole entry point called", "Information");

            while (true)
            {
                CloudQueueMessage message = queue.GetMessage();
                if (message == null)
                {
                    Trace.TraceInformation("Trenutno ne postoji poruka u redu.", "Information");
                }
                else
                {
                    Trace.TraceInformation(String.Format("Poruka glasi: {0}", message.AsString), "Information");

                    if (message.DequeueCount > 3)
                    {
                        queue.DeleteMessage(message);
                    }

                    if(message.AsString.Split('_').ElementAt(1) == "User")
                    {
                        ResizeImageUser(message.ToString().Split('_').ElementAt(0));
                    }
                    else if (message.AsString.Split('_').ElementAt(1) == "Topic")
                    {
                        ResizeImageTopic(message.AsString.Split('_').ElementAt(0));
                    }

                    Trace.TraceInformation(String.Format("Poruka procesuirana: {0}", message.AsString), "Information");
                }

                Thread.Sleep(5000);
                Trace.TraceInformation("Working", "Information");
            }

        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("ImageConverter_WorkerRole has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("ImageConverter_WorkerRole is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("ImageConverter_WorkerRole has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// Metoda pronalazi studenta po prosledjenom broju indeksa. Ukoliko student ne postoji, ispisuje poruku o tome u compute emulatoru.
        /// Ukoliko student postoji, preuzima sliku, konvertuje je u manju sliku i vrsi upload manje slike.
        /// cuva url do manje slike na mesto ThumbnailUrl.
        /// </summary>
        /// <param name="indexNo">broj indeksa studenta</param>
        public void ResizeImageUser(String indexNo)
        {
            UserDataRepository udr = new UserDataRepository();
            User user = udr.GetUser(indexNo);
            if (user == null)
            {
                Trace.TraceInformation(String.Format("Korisnik sa datim ID-em {0} ne postoji!", indexNo), "Information");
                return;
            }

            BlobHelper blobHelper = new BlobHelper();
            string uniqueBlobName = string.Format("image_{0}", user.RowKey);


            Image image = blobHelper.DownloadImage("reddit", uniqueBlobName);
            image = ImageConvertes.ConvertImage(image);
            string thumbnailUrl = blobHelper.UploadImage(image, "reddit", uniqueBlobName + "thumb");

            user.ThumbnailUrl = thumbnailUrl;
            udr.UpdateUser(user);
        }

        /// <summary>
        /// Metoda pronalazi studenta po prosledjenom broju indeksa. Ukoliko student ne postoji, ispisuje poruku o tome u compute emulatoru.
        /// Ukoliko student postoji, preuzima sliku, konvertuje je u manju sliku i vrsi upload manje slike.
        /// cuva url do manje slike na mesto ThumbnailUrl.
        /// </summary>
        /// <param name="indexNo">broj indeksa studenta</param>
        public void ResizeImageTopic(String indexNo)
        {
            TopicDataRepository tdr = new TopicDataRepository();
            Topic topic = tdr.GetTopic(indexNo);
            if (topic == null)
            {
                Trace.TraceInformation(String.Format("Korisnik sa datim ID-em {0} ne postoji!", indexNo), "Information");
                return;
            }

            BlobHelper blobHelper = new BlobHelper();
            string uniqueBlobName = string.Format("image_{0}", topic.RowKey);


            Image image = blobHelper.DownloadImage("reddit", uniqueBlobName);
            image = ImageConvertes.ConvertImage(image);
            string thumbnailUrl = blobHelper.UploadImage(image, "reddit", uniqueBlobName + "thumb");

            topic.ThumbnailUrl = thumbnailUrl;
            tdr.UpdateTopic(topic);
        }
    }
}
