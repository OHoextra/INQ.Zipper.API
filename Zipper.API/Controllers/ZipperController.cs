using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;
using Zipper.Application;
using Zipper.Model;
using Zipper.Application.Extensions;

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
        public async Task<IActionResult> ZipFilesAsync(IEnumerable<IFormFile> files, string? name = "", string compression = "Optimal")
        {
            try
            {
                if (!files.Any())
                    return BadRequest($"No files were found in the input collection. (Paramater: '{nameof(files)}')");

                var filesToZip = await Task.WhenAll(files.Select(file => file.ToFileDataAsync()));
                var zipFile = await ZipperService.ZipAsync(new ZipRequest(filesToZip, name ?? "", compression));
           
                return File(zipFile.Bytes, zipFile.ContentType, zipFile.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                return StatusCode(StatusCodes.Status500InternalServerError, _hostingEnvironment.IsDevelopment() ? ex.Message : null);
            }
        }

        /// <summary>Unzips the input file and returns a file or model (or route?)(TEST!) containing a collection of files. </summary>
        /// <remarks>Available compression types: <b>'Optimal'</b>, <b>'Fastest'</b>, <b>'SmallestSize'</b>.</remarks>
        [HttpPost]
        [Produces(MediaTypeNames.Application.Octet, MediaTypeNames.Text.Plain)]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(FileContentResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, type: typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(string))]
        public async Task<IActionResult> UnzipStreamAsync(IFormFile formFile, string location)
        {
            try
            {
                if (formFile == null)
                    return BadRequest($"'{nameof(formFile)}' is null.");

                var directoryFile = await ZipperService.UnzipAsync(
                    new UnzipRequest
                    {
                        ZipStream = formFile.OpenReadStream(),
                        OutPath = location
                    });

                return File(directoryFile.Bytes, directoryFile.ContentType, directoryFile.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                return StatusCode(StatusCodes.Status500InternalServerError, _hostingEnvironment.IsDevelopment() ? ex.Message : null);
            }
        }

    }
}