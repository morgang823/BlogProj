using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;



namespace BlogProj.Services
{
    public class BasicFileService : IBlogFileService
    {
        private const int DefaultMaxFileSize = (2 * 1024 * 1024);
        public string ContentType(IFormFile file)
        {
            return file?.ContentType.Split('/')[1];
            
        }

        public string DecodeImage(byte[] data, string type)
        {
            if (data is null || type is null) return null;
            return $"data:{type};base64,{Convert.ToBase64String(data)}";
        }

        public async Task<byte[]> EncodeFileAsync(IFormFile file)
        {
            if (file is null) return null;
            //Triggers more aggressive garbage collection
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            return ms.ToArray();
        }

        public async Task<byte[]> EncodeFileAsync(string fileName)
        {
            var file = $"{Directory.GetCurrentDirectory()}/wwwroot/img/{fileName}";
            return await File.ReadAllBytesAsync(file);
        }

        public int Size(IFormFile file)
        {
            return Convert.ToInt32(file?.Length);
        }

        public bool ValidateFileSize(IFormFile file)
        {
            return Size(file) < DefaultMaxFileSize;
        }

        public bool ValidateFileSize(IFormFile file, int maxSize)
        {
            return Size(file) < maxSize;
        }

        public bool ValidateFileType(IFormFile file)
        {
            var authorizedTypes = new List<string> { "jpg", "jpeg", "png", "gif" };
            var validExt = authorizedTypes.Contains(ContentType(file));
            return validExt;
        }
        
        public bool ValidateFileType(IFormFile file, List<string> fileType)
        {
            var validExt = fileType.Contains(ContentType(file));
            return validExt;
        }
    }
}
