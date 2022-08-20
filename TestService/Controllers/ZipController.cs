using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using ZipService.Application;
using ZipService.Model;

namespace TestService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ZipController : ControllerBase
    {
        private readonly ILogger<ZipController> _logger;

        public ZipController(ILogger<ZipController> logger)
        {
            _logger = logger;
        }

        private async Task<FileContent> ReadFormFile(IFormFile formFile)
        {
                byte[] fileBytes;
                using (var data = new MemoryStream())
                {
                    await formFile.CopyToAsync(data);
                    fileBytes = data.ToArray();
                }
                return new FileContent
                {
                    FileName = formFile.FileName,
                    FileBytes = fileBytes
                };
        }

        [HttpPost] // compressionTypes -> optimal, smallestsize, fastest
        public async Task<IActionResult> ZipFiles(IEnumerable<IFormFile> formFiles, string fileName, string compression = "FASTEST") // multiple iformfile
        {
            try
            {
                var zipFile = await FileZipper.ZipFiles(
                    formFiles.Select(f => 
                    ReadFormFile(f).Result), 
                    fileName, 
                    compression);

            
                    return File(zipFile.FileBytes, "application/zip", zipFile.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Environment.NewLine + ex.StackTrace, ex);
            }

            return StatusCode(500);
        }
    }
}