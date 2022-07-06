using FilesToAzure.Models;

namespace FilesToAzure.Repository
{
    public interface IFileRepository
    {
        Task<FileDto> GetFile(string fileName);
        Task<bool> UpdateFilePath(string filePath, string Id);
    }
}
