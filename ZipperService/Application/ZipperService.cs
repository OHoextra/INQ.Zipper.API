using System.IO.Compression;
using ZipService.Model;

namespace ZipService.Application
{
    public static class ZipperService
    {
        public static async Task<FileData> ZipFilesAsync(ZipFilesRequest request)
        {
            if (string.IsNullOrEmpty(request.ZipFileName))
                request.ZipFileName = DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + "_ZipArchive";

            CompressionLevel compressionLevel;
            switch (request.Compression.ToUpper().Trim(' '))
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
                    foreach (var file in request.FilesToZip)
                    {
                        var entry = archive.CreateEntry(file.FileName, compressionLevel);
                        using (var zipStream = entry.Open())
                        using (var fileStream = new MemoryStream(file.FileBytes))
                            await fileStream.CopyToAsync(zipStream);
                    }
                }
                zipFileBytes = ms.ToArray();
            }

            return new FileData
            {
                FileName = request.ZipFileName + ".zip",
                FileBytes = zipFileBytes
            };
        }
    }
}
