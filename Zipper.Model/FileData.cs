using System.Net.Mime;

namespace Zipper.Model
{

    public class FileData
    {
        public FileData(string contentType = "") 
        {
            ContentType = contentType;
        }
        public FileData(string name, byte[] bytes, string contentType = "") : this(contentType)
        {
            Name = name;
            Bytes = bytes;
        }

        public string? ContentType { get; set; } = MediaTypeNames.Application.Octet;

        public string? Name { get; set; }

        public byte[]? Bytes { get; set; }
    }
}
