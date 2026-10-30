using ImageMagick;

namespace Powerplant.FileFormats.ImageSharp;

public class QoiFileFormat : MagickFileFormat
{
    public override MagickFormat ImageFormat => MagickFormat.Qoi;
    
    public QoiFileFormat() : base("Quite OK Image Format (*.qoi)", ["qoi"])
    {
        
    }
}