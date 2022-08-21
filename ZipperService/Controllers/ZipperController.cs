using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ZipService.Application;
using ZipService.Model;
using System.Net.Mime;

namespace TestService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ZipperController : ControllerBase
    {
        private readonly ILogger<ZipperController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;

        /// <summary>The controller for the zipping and unzipping services. </summary>
        public ZipperController(ILogger<ZipperController> logger, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>Zips the input files and returns a Zip file on succesful completion. </summary>
        /// <remarks>Available compression types: <b>'Optimal'</b>, <b>'Fastest'</b>, <b>'SmallestSize'</b>.</remarks>
        [HttpPost]
        [Produces(MediaTypeNames.Application.Zip, MediaTypeNames.Text.Plain)]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(FileContentResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, type: typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(string))]
        public async Task<IActionResult> ZipFilesAsync(IEnumerable<IFormFile> filesToZip, string? zipFileName = "", string compression = "Optimal")
        {
            try
            {
                if (!filesToZip.Any())
                    return BadRequest($"No files were found in the input collection. (Paramater: '{nameof(filesToZip)}')");

                var zipFile = await ZipperService.ZipFilesAsync(
                    new ZipFilesRequest
                    {
                        ZipFileName = zipFileName,
                        Compression = compression,
                        FilesToZip = filesToZip.Select(
                            f => 
                            ReadFormFile(f).Result)
                        .ToList()
                    });

                    return File(zipFile.FileBytes, MediaTypeNames.Application.Zip, zipFile.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                if (_hostingEnvironment.IsDevelopment())
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
        }

        private async Task<FileData> ReadFormFile(IFormFile file)
        {
            byte[] fileBytes;
            using (var data = new MemoryStream())
            {
                await file.CopyToAsync(data);
                fileBytes = data.ToArray();
            }
            return new FileData
            {
                FileName = file.FileName,
                FileBytes = fileBytes
            };
        }
    }
}