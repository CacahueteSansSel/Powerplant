using ImageMagick;

namespace Powerplant.FileFormats.ImageSharp;

public class JpegFileFormat : MagickFileFormat
{
    public override MagickFormat ImageFormat => MagickFormat.Jpg;
    
    public JpegFileFormat() : base("JPEG Image (*.jpg)", ["jpg"])
    {
        
    }
}