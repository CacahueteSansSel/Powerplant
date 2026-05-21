using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Png.Chunks;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;

namespace Powerplant.FileFormats.ImageSharp;

public class WebPFileFormat : ImageSharpFileFormat
{
    public WebPFileFormat() : base("WebP (*.webp)", ["webp"])
    {
        
    }

    protected override void SaveInternal(Image<Rgba32> image, string filename)
    {
        image.SaveAsWebp(filename);
    }
}