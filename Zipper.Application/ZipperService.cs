using System.IO.Compression;
using System.Net.Mime;
using Zipper.Model;

namespace Zipper.Application
{
    public class ZipperService
    {
        public static async Task<FileData> ZipAsync(IEnumerable<FileData> filesToZip, string name = "", string compression = "Fastest")
        {
            if (!filesToZip.Any())
                throw new ArgumentException("There are no files to zip!");

            if (string.IsNullOrEmpty(name))
                name = DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + "_ZipArchive";

            CompressionLevel compressionLevel;
            switch (compression?.ToUpper().Trim(' '))
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

            byte[] bytes;
            using (var archiveStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, leaveOpen: true))
                {
                     await Task.WhenAll(filesToZip.Select(
                            async fileToZip => 
                            {
                                var entry = archive.CreateEntry(name, compressionLevel);

                                using var entryStream = entry.Open();
                                using var fileStream = new MemoryStream(fileToZip.Bytes);

                                await fileStream.CopyToAsync(entryStream);

                                return entry;
                            }));
                }
                bytes = archiveStream.ToArray();
            }

            return new FileData(name + ".zip", bytes, MediaTypeNames.Application.Zip);
        }

        public async Task<IEnumerable<FileData>> UnzipAsync(Stream zipStream)
        {
            var zipArchive = new ZipArchive(zipStream);

            return await Task.WhenAll(zipArchive.Entries.Select(
                    async entry =>
                    {
                        using var entryStream = new MemoryStream();
                        await entry.Open().CopyToAsync(entryStream);
                        return new FileData(entry.Name, entryStream.ToArray());
                    }));
        }
        
    }
}
