using Microsoft.AspNetCore.Http;

namespace Zipper.Model.Extensions
{
    public static class FormFileExtensions
    {
        public static async Task<FileData> ToFileDataAsync(this IFormFile formFile)
        {
            using var memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);

            return new FileData(formFile.FileName, memoryStream.ToArray());
        }
    }
}
