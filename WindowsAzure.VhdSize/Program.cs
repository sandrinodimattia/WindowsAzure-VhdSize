using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WindowsAzure.VhdSize
{
    class Program
    {
        static void Main(string[] args)
        {
            // Header.
            Console.WriteLine("");
            Console.WriteLine(" Windows Azure VHD Size v1.0 (2013-12-18) - Sandrino Di Mattia");
            Console.WriteLine(" Usage: wazvhdsize.exe <accountName> <accountKey> <vhdUrl|containerName>");
            Console.WriteLine("");

            // Validate args.
            if (args.Length != 3)
            {
                Console.WriteLine(" Invalid number of arguments.");
                Console.WriteLine("");
                return;
            }

            // Get the uri.
            var uri = args[2];
            Console.WriteLine(" Processing: {0}", uri);
            Console.WriteLine("");

            try
            {
                // Init client and blob list.
                var client = new CloudStorageAccount(new StorageCredentials(args[0], args[1]), false).CreateCloudBlobClient();
                var isBlobUri = false;
                var blobs = new List<CloudPageBlob>();

                // It's an uri.
                if (uri.StartsWith("http://") || uri.StartsWith("https://"))
                {
                    var blob = client.GetBlobReferenceFromServer(new Uri(uri)) as CloudPageBlob;
                    if (blob == null)
                        throw new FileNotFoundException("Unable to find the Page Blob.");
                    blobs.Add(blob);
                    isBlobUri = true;
                }
                else
                {
                    // It's a container.
                    var container = client.GetContainerReference(uri);
                    if (!container.Exists())
                        throw new InvalidOperationException("Container does not exist: " + uri);
                    blobs.AddRange(container.ListBlobs().OfType<CloudPageBlob>());
                }

                // Show blob sizes.
                foreach (var blob in blobs)
                {
                    if (!isBlobUri)
                        Console.WriteLine(" Blob: {0}", blob.Uri);

                    // Display length.
                    Console.WriteLine(" > Size: {0} ({1} bytes)", PageBlobExtensions.GetFormattedDiskSize(blob.Properties.Length), blob.Properties.Length);

                    // Calculate size.
                    var size = blob.GetActualDiskSize();
                    Console.WriteLine(" > Actual/Billable Size: {0} ({1} bytes)", PageBlobExtensions.GetFormattedDiskSize(size), size);
                    Console.WriteLine("");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(" Error:");
                Console.WriteLine(" {0}", ex);
            }
        }
    }
}
