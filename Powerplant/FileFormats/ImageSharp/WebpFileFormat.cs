using ImageMagick;

namespace Powerplant.FileFormats.ImageSharp;

public class WebPFileFormat : MagickFileFormat
{
    public override MagickFormat ImageFormat => MagickFormat.WebP;
    
    public WebPFileFormat() : base("WebP (*.webp)", ["webp"])
    {
        
    }
}