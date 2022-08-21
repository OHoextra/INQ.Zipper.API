using Microsoft.AspNetCore.Http;

namespace Zipper.Model
{
    public class ZipRequest
    {
        public ZipRequest() { }
        public ZipRequest(IEnumerable<FileData> filesToZip, string name = "", string compression = "Fastest")
        {
            FilesToZip = filesToZip;
            Name = name;
            Compression = compression;
        }

        public IEnumerable<FileData> FilesToZip { get; set; } = new List<FileData>();

        public string? Name { get; set; } = "";

        public string? Compression { get; set; } = "FASTEST";
    }
}
