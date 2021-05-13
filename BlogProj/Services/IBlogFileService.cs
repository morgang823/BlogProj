using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogProj.Services
{
   public interface IBlogFileService
    {
        //encode an image from an upload control
        Task<byte[]> EncodeFileAsync(IFormFile file);

        //ENCODE AN IMAGE FROM A URL

        Task<byte[]> EncodeFileAsync(string fileName);
        //Pulls byte array out of database allow us to set it equal to string
        string DecodeImage(byte[] data, string type);

        bool ValidateFileType(IFormFile file);
        bool ValidateFileType(IFormFile file, List<string> fileType);

        bool ValidateFileSize(IFormFile file);
        bool ValidateFileSize(IFormFile file, int maxSize);

        string ContentType(IFormFile file);
        int Size(IFormFile file);

    }
}
