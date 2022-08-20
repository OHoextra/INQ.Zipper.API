using System.IO.Compression;
using ZipService.Model;

namespace ZipService.Application
{
    public static class FileZipper
    {
        public static async Task<FileContent> ZipFiles(IEnumerable<FileContent> files, string fileName, string compression = "FASTEST") // multiple iformfile
        {
            if (string.IsNullOrEmpty(fileName))
                fileName = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + "_ZipArchive";

            CompressionLevel compressionLevel;
            switch (compression.ToUpper().Trim(' '))
            {
                case "OPTIMAL":
                    compressionLevel = CompressionLevel.Optimal;
                    break;

                case "SMALLESTSIZE":
                    compressionLevel = CompressionLevel.SmallestSize;
                    break;

                default:
                    compressionLevel = CompressionLevel.Fastest;
                    break;
            }

            byte[] zipFileBytes;
            using (var ms = new MemoryStream())
            {
                using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
                {
                    foreach (var file in files)
                    {
                        var entry = archive.CreateEntry(file.FileName, compressionLevel);
                        using (var zipStream = entry.Open())
                        using (var fileStream = new MemoryStream(file.FileBytes))
                            await fileStream.CopyToAsync(zipStream);
                    }
                }
                zipFileBytes = ms.ToArray();
            }

            return new FileContent
            {
                FileName = fileName + ".zip",
                FileBytes = zipFileBytes
            };
        }
    }
}
