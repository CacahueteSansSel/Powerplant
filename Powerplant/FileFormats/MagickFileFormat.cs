using System.Xml.Linq;
using ImageMagick;
using Powerplant.Core;

namespace Powerplant.FileFormats;

public abstract class MagickFileFormat : FileFormatBase
{
    public override string Name { get; }
    public override string[] Extensions { get; }
    
    public abstract MagickFormat ImageFormat { get; }

    public MagickFileFormat(string name, string[] extensions)
    {
        Name = name;
        Extensions = extensions;
    }

    public override void Save(ViewportBitmap bitmap, string filename)
    {
        using MagickImage img = new(MagickColors.Transparent, (uint)bitmap.Width, (uint)bitmap.Height);
        using IPixelCollection<byte> pixels = img.GetPixels();
        
        img.Alpha(AlphaAction.Set);

        for (int y = 0; y < img.Height; y++)
        {
            for (int x = 0; x < img.Width; x++)
            {
                PwColor color = bitmap.Get(x, y);
                
                pixels.SetPixel(x, y, [color.R, color.G, color.B, color.A]);
            }
        }
        
        img.SetAttribute("Software", "Powerplant");
        
        img.Write(filename, ImageFormat);
    }

    public override ViewportBitmap? Load(string filename)
    {
        using MagickImage img = new MagickImage(filename);
        using IPixelCollection<byte> pixels = img.GetPixels();

        ViewportBitmap bmp = new((int)img.Width, (int)img.Height);

        for (int y = 0; y < img.Height; y++)
        {
            for (int x = 0; x < img.Width; x++)
            {
                IPixel<byte> color = pixels.GetPixel(x, y);
                byte alpha = img.IsOpaque ? (byte)255 : color.GetChannel(3);
                
                bmp.Set(x, y, new PwColor(color.GetChannel(0), color.GetChannel(1), 
                    color.GetChannel(2), alpha), false);
            }
        }
        
        return bmp;
    }
}