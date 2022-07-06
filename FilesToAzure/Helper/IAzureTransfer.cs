namespace FilesToAzure.Helper
{
    public interface IAzureTransfer
    {
        Task<string> Upload(IFormFile data);
    }
}
