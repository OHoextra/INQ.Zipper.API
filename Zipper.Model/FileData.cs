using Microsoft.AspNetCore.StaticFiles;
using System.Net.Mime;

namespace Zipper.Model
{

    public class FileData
    {
        public FileData(string name, byte[] bytes, string contentType = "")
        {
            Name = name;
            Bytes = bytes;
            ContentType = string.IsNullOrEmpty(contentType) ? GetMimeType(name) : contentType;
        }

        public string? ContentType { get; set; }

        public string? Name { get; set; }

        public byte[]? Bytes { get; set; }

        private static string GetMimeType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(fileName, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}
