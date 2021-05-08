using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Hosting;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Collections.Generic;
using System.Linq;

namespace app_task_azureweekend
{
    public class Service : BackgroundService
    {
        public Service()
        {
            Console.WriteLine("CTOR SERVICE");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                Console.WriteLine("CONNECTING BLOB");
                string containerEndpoint = string.Format("https://{0}.blob.core.windows.net/{1}",
                                                    "storageazureweekend",
                                                    "inputtaskcontainer");
                var blobContainerClient = new BlobContainerClient(new Uri(containerEndpoint),
                                                                        new DefaultAzureCredential());

                while (!stoppingToken.IsCancellationRequested)
                {
                    Console.WriteLine("THREADING RUNNING...");

                    var resultSegment = blobContainerClient.GetBlobsAsync()
                        .AsPages(default, 5);

                    // Enumerate the blobs returned for each page.
                    await foreach (Azure.Page<BlobItem> blobPage in resultSegment)
                    {
                        foreach (BlobItem blobItem in blobPage.Values)
                        {
                            var blobClient = blobContainerClient.GetBlobClient(blobItem.Name);
                            var properties = await blobClient.GetPropertiesAsync();
                            var hasProperty = properties.Value.Metadata.Where(p => p.Key.Equals("processed") & p.Value.Equals("1")).Count();

                            if(hasProperty == 1 || blobItem.Name.Contains("output")){
                                 Console.WriteLine("PROCESSING DONE: {0}", blobItem.Name);
                                continue;
                            }
                            
                            Console.WriteLine("PROCESSING BLOB: {0}", blobItem.Name);

                            IDictionary<string, string> metadata = new Dictionary<string, string>();
                            metadata.Add("processed", "1");
                            await blobClient.SetMetadataAsync(metadata);

                            using (MemoryStream ms = new MemoryStream())
                            {
                                
                                await blobClient.DownloadToAsync(ms);
                                ms.Position = 0;

                                using (Image image = Image.Load(ms))
                                {
                                    int width = image.Width / 2;
                                    int height = image.Height / 2;
                                    image.Mutate(x => x.Resize(width, height));
                                    
                                    using (MemoryStream msresize = new MemoryStream())
                                    {
                                        image.SaveAsJpeg(msresize);
                                        msresize.Position = 0;
                                        var redizeBlobClient = blobContainerClient.GetBlobClient(blobItem.Name.Replace("input","output"));
                                        await redizeBlobClient.UploadAsync(msresize,true);
                                    }
                                }
                                
                            }
                        }

                        Console.WriteLine();
                    }

                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}