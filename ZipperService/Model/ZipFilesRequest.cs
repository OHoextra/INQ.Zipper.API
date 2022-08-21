namespace ZipService.Model
{
    public class ZipFilesRequest
    {
        public List<FileData> FilesToZip { get; set; } = new List<FileData>();
        
        public string ZipFileName { get; set; }

        public string Compression { get; set; } = "FASTEST";
    }
}
