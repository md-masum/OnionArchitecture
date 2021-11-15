using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.Exceptions;
using Core.Interfaces.Common;
using Microsoft.AspNetCore.Http;

namespace Core.Services
{
    public class FileUploadService : IFileUploadService
    {
        public bool DeleteFile(string key)
        {
            var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\" + key);
            FileInfo file = new FileInfo(pathBuilt);
            if (file.Exists)
            {
                file.Delete();
                return true;
            }

            return false;
        }

        public async Task<string> UploadFile(string keyPrefix, IFormFile file)
        {
            try
            {
                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                var fileName = keyPrefix + extension;

                var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Upload\\files");

                if (!Directory.Exists(pathBuilt))
                {
                    Directory.CreateDirectory(pathBuilt);
                }

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Upload\\files",
                    fileName);

                await using var stream = new FileStream(path, FileMode.Create);
                await file.CopyToAsync(stream);

                return Path.Combine("Upload\\files", fileName);
            }
            catch (Exception e)
            {
                throw new CustomException($"Failed to save file, Message: {e.Message}");
            }
        }

        public async Task<List<string>> UploadFiles(List<(string keyPrefix, IFormFile file)> uploadFiles)
        {
            List<string> fileName = new List<string>();

            foreach (var uploadFile in uploadFiles)
            {
                fileName.Add(await UploadFile(uploadFile.keyPrefix, uploadFile.file));
            }

            return fileName;
        }
    }
}
