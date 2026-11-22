using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Passalong.Api.Services;

namespace Passalong.Api.Controllers;

[ApiController]
[Route("api/images")]
public class ImagesController(ImageUploadService uploads) : ApiControllerBase
{
    private const int MaxFiles = 6;

    /// <summary>Uploads up to six pictures (multipart field "files") and returns their URLs.</summary>
    [Authorize]
    [HttpPost]
    [RequestSizeLimit(ImageUploadService.MaxBytes * MaxFiles + 1024 * 1024)]
    public async Task<IActionResult> Upload([FromForm] List<IFormFile> files, CancellationToken ct)
    {
        if (files.Count == 0)
        {
            return BadRequest("Choose at least one picture.");
        }

        if (files.Count > MaxFiles)
        {
            return BadRequest($"You can upload at most {MaxFiles} pictures.");
        }

        var urls = new List<string>();

        foreach (var file in files)
        {
            if (file.Length is 0 or > ImageUploadService.MaxBytes)
            {
                return BadRequest($"\"{file.FileName}\" must be smaller than {ImageUploadService.MaxBytes / 1024 / 1024} MB.");
            }

            var url = await uploads.SaveAsync(file, ct);

            if (url is null)
            {
                return BadRequest($"\"{file.FileName}\" is not a valid picture.");
            }

            urls.Add(url);
        }

        return Ok(new { urls });
    }
}
