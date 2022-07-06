using Dapper;
using FilesToAzure.Models;
using System.Data.SqlClient;

namespace FilesToAzure.Repository
{
    public class FileRepository : IFileRepository
    {
        private readonly IConfiguration _configuration;

        public FileRepository(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }


        public async Task<FileDto> GetFile(string fileName)
        {
            using (var conn = new SqlConnection(_configuration.GetConnectionString("Mssql")))
            {
                var sql = $"select Name,Id from [Case].Files_OLD where Name = N'{fileName}'";
                return await conn.QueryFirstOrDefaultAsync<FileDto>(sql);
            }
        }

        public async Task<bool> UpdateFilePath(string filePath, string Id)
        {
            try
            {
                using (var conn = new SqlConnection(_configuration.GetConnectionString("Mssql")))
                {
                    var sql = $"update [Case].Files_OLD set Path = N'{filePath}' where Id = '{Id}'";
                    await conn.ExecuteAsync(sql);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
