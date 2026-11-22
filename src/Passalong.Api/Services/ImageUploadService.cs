using SkiaSharp;

namespace Passalong.Api.Services;

/// <summary>
/// Validates uploaded pictures, downsizes them and stores them on disk as JPEG.
/// Re-encoding guarantees the stored file really is an image and strips metadata such as GPS tags.
/// </summary>
public class ImageUploadService
{
    public const string RequestPath = "/api/uploads";
    public const long MaxBytes = 5 * 1024 * 1024;
    private const int MaxDimension = 1600;

    private readonly string _root;

    public ImageUploadService(IWebHostEnvironment env, IConfiguration configuration)
    {
        _root = Path.GetFullPath(configuration["Uploads:Path"] ?? Path.Combine(env.ContentRootPath, "uploads"));
        Directory.CreateDirectory(_root);
    }

    public string RootPath => _root;

    /// <summary>Returns the public URL of the stored image, or null when the file is not a valid image.</summary>
    public async Task<string?> SaveAsync(IFormFile file, CancellationToken ct)
    {
        await using var input = file.OpenReadStream();
        using var buffer = new MemoryStream();
        await input.CopyToAsync(buffer, ct);
        buffer.Position = 0;

        using var codec = SKCodec.Create(buffer);

        if (codec is null)
        {
            return null;
        }

        using var decoded = SKBitmap.Decode(codec);

        if (decoded is null)
        {
            return null;
        }

        using var oriented = ApplyOrientation(decoded, codec.EncodedOrigin);
        using var resized = Downsize(oriented);
        using var data = SKImage.FromBitmap(resized).Encode(SKEncodedImageFormat.Jpeg, 85);

        var name = $"{Guid.NewGuid():N}.jpg";
        await using var output = File.Create(Path.Combine(_root, name));
        data.SaveTo(output);

        return $"{RequestPath}/{name}";
    }

    private static SKBitmap Downsize(SKBitmap source)
    {
        var scale = Math.Min(1f, (float)MaxDimension / Math.Max(source.Width, source.Height));

        if (scale >= 1f)
        {
            return source.Copy();
        }

        var info = new SKImageInfo((int)(source.Width * scale), (int)(source.Height * scale));

        return source.Resize(info, new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear))
               ?? source.Copy();
    }

    /// <summary>Re-encoding drops EXIF data, so rotate the pixels the way the camera meant them to be shown.</summary>
    private static SKBitmap ApplyOrientation(SKBitmap source, SKEncodedOrigin origin)
    {
        var degrees = origin switch
        {
            SKEncodedOrigin.RightTop => 90,
            SKEncodedOrigin.BottomRight => 180,
            SKEncodedOrigin.LeftBottom => 270,
            _ => 0
        };

        if (degrees == 0)
        {
            return source.Copy();
        }

        var swap = degrees != 180;
        var rotated = new SKBitmap(swap ? source.Height : source.Width, swap ? source.Width : source.Height);

        using var canvas = new SKCanvas(rotated);
        canvas.Translate(rotated.Width / 2f, rotated.Height / 2f);
        canvas.RotateDegrees(degrees);
        canvas.Translate(-source.Width / 2f, -source.Height / 2f);
        using var image = SKImage.FromBitmap(source);
        canvas.DrawImage(image, 0, 0, new SKSamplingOptions(SKFilterMode.Linear), null);

        return rotated;
    }
}
