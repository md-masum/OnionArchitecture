using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Core.Interfaces.Common
{
    public interface IFileUploadService
    {
        bool DeleteFile(string key);
        Task<string> UploadFile(string keyPrefix, IFormFile file);
        Task<List<string>> UploadFiles(List<(string keyPrefix, IFormFile file)> uploadFiles);
    }
}