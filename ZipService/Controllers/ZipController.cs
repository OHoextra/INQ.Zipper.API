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
        /// <summary>Zips the input files and returns a Zip file on succesful completion. </summary>
        /// <remarks>Available compression types: <b>'Optimal'</b>, <b>'Fastest'</b>, <b>'SmallestSize'</b>.</remarks>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
        public async Task<IActionResult> ZipFiles(IEnumerable<IFormFile> inputFiles, string zipFileName, string compression = "Fastest")
        {
            try
            {
                if (inputFiles == null)
                    return BadRequest("inputFiles == null");

                if (!inputFiles.Any())
                    return BadRequest("no files in input.");

                var zipFile = await FileZipper.ZipFiles(
                    inputFiles.Select(f =>
                    ReadFormFile(f).Result),
                    zipFileName,
                    compression);

                    return Ok(File(zipFile.FileBytes, "application/zip", zipFile.FileName));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}