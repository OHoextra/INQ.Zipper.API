using System.IO.Compression;
using Zipper.Model;

namespace Zipper.Application
{
    public class ZipperService
    {
        ///<exception cref = "ArgumentNullException"></exception>
        ///<exception cref = "ArgumentException"></exception>
        ///<exception cref = "InvalidOperationException"></exception>
        public static async Task<FileData> ZipAsync(IEnumerable<FileData> filesToZip, string name = "", string compression = "")
        {
            if (filesToZip == null)
                throw new ArgumentNullException(nameof(filesToZip));

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

            try
            {
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

                return new FileData(name + ".zip", bytes);
            }
            catch (Exception ex)
            {
                if (ex is InvalidDataException)
                    throw new ArgumentException($"One of the input files in parameter: '{nameof(filesToZip)}' contain invalid data.");

                if (ex is InvalidOperationException)
                    throw;

                // If we can't relate it to the Arguments, simplify output exception
                throw new InvalidOperationException(ex.Message);
            }
        }

        ///<exception cref = "ArgumentNullException"></exception>
        ///<exception cref = "ArgumentException"></exception>
        ///<exception cref = "InvalidOperationException"></exception>
        public async Task<IEnumerable<FileData>> UnzipAsync(Stream zipStream)
        {
            if (zipStream == null)
                throw new ArgumentNullException(nameof(zipStream));

            try
            {
                var zipArchive = new ZipArchive(zipStream);
                var unzippedFiles = await Task.WhenAll(zipArchive.Entries.Select(
                        async entry =>
                        {
                            var fileName = entry.Name;
                            var fileContentType = FileHelper.GetContentType(fileName);
                            using var entryStream = new MemoryStream();
                            await entry.Open().CopyToAsync(entryStream);
                            return new FileData(fileName, entryStream.ToArray(), fileContentType);
                        }));

                return unzippedFiles;
            }
            catch (Exception ex)
            {
                if (ex is InvalidDataException)
                    throw new ArgumentException($"Parameter: '{nameof(zipStream)}' contain invalid data.");

                if (ex is InvalidOperationException)
                    throw;

                // If we can't relate it to the Arguments, simplify output exception
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
