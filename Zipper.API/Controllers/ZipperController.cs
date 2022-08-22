using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;
using Zipper.Application;
using Zipper.Model;

namespace TestService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ZipperController : ControllerBase
    {
        private readonly ZipperService _zipperService;
        private readonly ILogger<ZipperController> _logger;
        private readonly IWebHostEnvironment _hostEnv;

        /// <summary>The controller for the zipping and unzipping services. </summary>
        public ZipperController(ZipperService zipperService, ILogger<ZipperController> logger, IWebHostEnvironment hostingEnvironment)
        {
            _zipperService = zipperService;
            _logger = logger;
            _hostEnv = hostingEnvironment;
        }

        /// <summary>Zips the input files and returns a Zip file on succesful completion. </summary>
        /// <remarks>Available compression types: <b>'Optimal'</b>, <b>'Fastest'</b>, <b>'SmallestSize'</b>.</remarks>
        [HttpPost]
        [Produces(MediaTypeNames.Application.Zip, MediaTypeNames.Text.Plain)]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(FileContentResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, type: typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(string))]
        public async Task<IActionResult> ZipAsync(IEnumerable<IFormFile> files, string? name = "", string compression = "Optimal")
        {
            try
            {
                if (!files.Any())
                    return BadRequest($"No files were found in the input collection. (Paramater: '{nameof(files)}')");

                var filesToZip = await Task.WhenAll(files.Select(
                    async file => 
                    {
                        using var memoryStream = new MemoryStream();
                        await file.CopyToAsync(memoryStream);

                        return new FileData(file.FileName, memoryStream.ToArray());
                    }));

                var zipFile = await ZipperService.ZipAsync(filesToZip, name ?? "", compression);
           
                return File(zipFile.Bytes, zipFile.ContentType, zipFile.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                return StatusCode(StatusCodes.Status500InternalServerError, _hostEnv.IsDevelopment() ? ex : null);
            }
        }

        /// <summary>Unzips the input file and returns a file or model (or route?)(TEST!) containing a collection of files. </summary>
        /// <remarks>Available compression types: <b>'Optimal'</b>, <b>'Fastest'</b>, <b>'SmallestSize'</b>.</remarks>
        [HttpPost]
        [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Text.Plain)]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(IEnumerable<FileData>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, type: typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(string))]
        public async Task<IActionResult> UnzipAsync(IFormFile formFile)
        {
            try
            {
                if (formFile == null)
                    return BadRequest($"'{nameof(formFile)}' is null.");

                var unzippedFiles = await _zipperService.UnzipAsync(formFile.OpenReadStream());

                return Ok(unzippedFiles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                return StatusCode(StatusCodes.Status500InternalServerError, _hostEnv.IsDevelopment() ? ex : null);
            }
        }

    }
}