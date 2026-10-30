using System.Web;
using ImageMagick;

namespace Powerplant.FileFormats.ImageSharp;

public class PngFileFormat : MagickFileFormat
{
    public override MagickFormat ImageFormat => MagickFormat.Png;
    
    public PngFileFormat() : base("Portable Network Graphics (*.png)", ["png"])
    {
        
    }
}