using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Png.Chunks;
using SixLabors.ImageSharp.PixelFormats;

namespace Powerplant.FileFormats.ImageSharp;

public class PngFileFormat : ImageSharpFileFormat
{
    public PngFileFormat() : base("PNG", ["png"])
    {
        
    }

    protected override void SaveInternal(Image<Rgba32> image, string filename)
    {
        PngMetadata pngMeta = image.Metadata.GetPngMetadata();
        pngMeta.TextData.Add(new PngTextData("Software", "Powerplant", "", ""));
        
        image.SaveAsPng(filename);
    }
}