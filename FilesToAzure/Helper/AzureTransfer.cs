using Microsoft.AspNetCore.StaticFiles;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace FilesToAzure.Helper
{
    public class AzureTransfer : IAzureTransfer
    {
        private readonly IConfiguration _configuration;

        public AzureTransfer(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<string> Upload(IFormFile data)
        {
            try
            {
                var provider = new FileExtensionContentTypeProvider();
                string contentType = string.Empty;

                string blobstorageconnection = _configuration.GetValue<string>("BlobConnectionString");
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobstorageconnection);
                CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(_configuration.GetValue<string>("BlobContainerName"));
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(data.FileName);

                provider.TryGetContentType(data.FileName, out contentType);
                if (!string.IsNullOrEmpty(contentType)) blockBlob.Properties.ContentType = contentType;

                await using (var item = data.OpenReadStream())
                {
                    await blockBlob.UploadFromStreamAsync(item);
                }
                return blockBlob.Uri.ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
