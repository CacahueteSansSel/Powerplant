using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Powerplant.FileFormats.ImageSharp;

public class PngFileFormat : ImageSharpFileFormat
{
    public PngFileFormat() : base("PNG", ["png"])
    {
        
    }

    protected override void SaveInternal(Image<Rgba32> image, string filename)
    {
        image.SaveAsPng(filename);
    }
}