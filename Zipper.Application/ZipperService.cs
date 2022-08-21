using System.IO.Compression;
using System.Net.Mime;
using Zipper.Model;

namespace Zipper.Application
{
    public static class ZipperService
    {
        public static async Task<FileData> ZipAsync(ZipRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrEmpty(request.Name))
                request.Name = DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + "_ZipArchive";

            CompressionLevel compressionLevel;
            switch (request.Compression?.ToUpper().Trim(' '))
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
                     await Task.WhenAll(
                         request.FilesToZip.Select(
                            async fileToZip =>
                            {
                                if (string.IsNullOrEmpty(fileToZip.Name))
                                    throw new ArgumentException($"{nameof(fileToZip)}.{nameof(fileToZip.Name)} is null or empty");

                                if (fileToZip.Bytes == null)
                                    throw new ArgumentException($"{nameof(fileToZip)}.{nameof(fileToZip.Bytes)} collection is null.");

                                var entry = archive.CreateEntry(fileToZip.Name, compressionLevel);
                                using var entryStream = entry.Open();
                                using var fileToZipStream = new MemoryStream(fileToZip.Bytes.ToArray());
                                await fileToZipStream.CopyToAsync(entryStream);
                            }));
                }
                bytes = archiveStream.ToArray();
            }

            return new FileData(request.Name + ".zip", bytes, MediaTypeNames.Application.Zip);
        }

        public static async Task<FileData> UnzipAsync(UnzipRequest request)
        {

            if (request.ZipStream == null)
                throw new ArgumentNullException(nameof(request.ZipStream));

            if (string.IsNullOrEmpty(request.OutPath))
                throw new ArgumentNullException(nameof(request.OutPath));


            var archive = new ZipArchive(request.ZipStream);
            Directory.CreateDirectory(request.OutPath);

            await Task.WhenAll(
                archive.Entries.Select(
                    async entry =>
                    {
                        var entryBytes = await ReadFileBytesAsync(entry.Open());
                        WriteFileBytes(Path.Combine(request.OutPath, entry.Name), entryBytes);
                    }));

            // TODO: likely we need to return a file array here
            return new FileData(request.OutPath, ReadFileBytes(request.OutPath), MediaTypeNames.Application.Octet);
        }
        private static void WriteFileBytes(string path, byte[] entryBytes)
        {
            var memoryStream = new MemoryStream(entryBytes);
            FileStream unzippedFileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            memoryStream.WriteTo(unzippedFileStream);
            unzippedFileStream.Close();
            memoryStream.Close();
        }
        private static byte[] ReadFileBytes(string path)
        {
            byte[] fileContent = null;
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fs);
            long byteLength = new FileInfo(path).Length;
            fileContent = binaryReader.ReadBytes((Int32)byteLength);
            fs.Close();
            fs.Dispose();
            binaryReader.Close();
            return fileContent;
        }

        private static async Task<byte[]> ReadFileBytesAsync(Stream stream)
        {
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

    }

}
