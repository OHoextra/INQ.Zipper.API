using Microsoft.AspNetCore.StaticFiles;
using System.Net.Mime;

namespace Zipper.Application
{
    public static class FileHelper
    {
        public static string GetContentType(string fileName = "")
        {
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".zip"] = MediaTypeNames.Application.Zip;

            string contentType = MediaTypeNames.Application.Octet;
            provider.TryGetContentType(fileName, out contentType);

            return contentType;
        }

        ///<exception cref = "ArgumentNullException"></exception>
        ///<exception cref = "ArgumentException"></exception>
        ///<exception cref = "InvalidOperationException"></exception>
        public static void ValidateFileName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            var fileName = Path.GetFileName(name);
            if (fileName != name)
                throw new ArgumentException($"Parameter '{nameof(name)}' may not include directories.");

            ValidateFilePath(name);
        }

        ///<exception cref = "ArgumentNullException"></exception>
        ///<exception cref = "ArgumentException"></exception>
        ///<exception cref = "InvalidOperationException"></exception>
        public static void ValidateFilePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            var fileName = Path.GetFileName(filePath);
            if (fileName.Length < filePath.Length)
                throw new ArgumentException($"Parameter '{nameof(filePath)}' may not include directories.");

            FileInfo? fi = null;
            try
            {
                fi = new FileInfo(filePath);
            }
            catch (Exception ex)
            {
                if (ex is PathTooLongException || ex is NotSupportedException)
                {
                    throw new ArgumentException(ex.Message);
                }
                if (ex is System.Security.SecurityException || ex is UnauthorizedAccessException)
                {
                    throw new InvalidOperationException(ex.Message);
                }

                throw;
            }
            if (ReferenceEquals(fi, null))
            {
                throw new InvalidOperationException($"Parameter '{nameof(filePath)}' was found to be invalid.");
            }
            else
            {
                if (fi.Exists)
                    throw new ArgumentException($"Parameter '{nameof(filePath)}' cannot be used, the file already exists.");
            }
        }
    }
}
