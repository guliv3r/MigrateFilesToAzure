using FilesToAzure.Helper;
using FilesToAzure.Repository;
using Microsoft.AspNetCore.Mvc;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace FilesToAzure.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MigrationController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly IAzureTransfer _azureTransfer;
        private readonly IFileRepository _fileRepository;

        public MigrationController(IWebHostEnvironment environment, IConfiguration configuration, IAzureTransfer azureTransfer, IFileRepository fileRepository)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(_configuration));
            _azureTransfer = azureTransfer ?? throw new ArgumentNullException(nameof(_azureTransfer));
            _fileRepository = fileRepository ?? throw new ArgumentNullException(nameof(fileRepository));
        }

        [HttpGet]
        public async Task<bool> Migrate()
        {
            string source = _configuration.GetValue<string>("SourceFilesPath");
            string[] filePaths = Directory.GetFiles(Path.Combine(source));

            
            foreach (string item in filePaths)
            {
                var arr = item.Split("\\");
                if (arr.Length == 0) continue;
                string fileName = arr[arr.Length - 1];

                var fileFromDb = await _fileRepository.GetFile(fileName);
                if (fileFromDb == null) continue;

                byte[] bytes = System.IO.File.ReadAllBytes(item);

                var stream = new MemoryStream(bytes);
                IFormFile file = new FormFile(stream, 0, bytes.Length, fileName, fileName);
                var res = await _azureTransfer.Upload(file);

                if (!string.IsNullOrEmpty(res)) await _fileRepository.UpdateFilePath(res, fileFromDb.Id.ToString());
            }
            return true;
        }
    }
}