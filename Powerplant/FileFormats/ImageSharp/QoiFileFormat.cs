using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Png.Chunks;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;

namespace Powerplant.FileFormats.ImageSharp;

public class QoiFileFormat : ImageSharpFileFormat
{
    public QoiFileFormat() : base("Quite OK Image Format (*.qoi)", ["qoi"])
    {
        
    }

    protected override void SaveInternal(Image<Rgba32> image, string filename)
    {
        image.SaveAsQoi(filename);
    }
}