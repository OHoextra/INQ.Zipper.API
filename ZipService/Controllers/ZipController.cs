using Microsoft.AspNetCore.Mvc;
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

        private async Task<FileContent> ReadFormFile(IFormFile file)
        {
                byte[] fileBytes;
                using (var data = new MemoryStream())
                {
                    await file.CopyToAsync(data);
                    fileBytes = data.ToArray();
                }
                return new FileContent
                {
                    FileName = file.FileName,
                    FileBytes = fileBytes
                };
        }
        /// <summary>Zips files. </summary>
        /// <remarks>Available compression types: 'Optimal', 'Fastest', 'SmallestSize'.</remarks>
        [HttpPost]
        public async Task<IActionResult> ZipFiles(IEnumerable<IFormFile> files, string zipFileName, string compression = "Fastest")
        {
            try
            {
                var zipFile = await FileZipper.ZipFiles(
                    files.Select(f =>
                    ReadFormFile(f).Result),
                    zipFileName,
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