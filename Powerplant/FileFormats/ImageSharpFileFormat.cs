using Powerplant.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Powerplant.FileFormats;

public abstract class ImageSharpFileFormat : FileFormatBase
{
    public override string Name { get; }
    public override string[] Extensions { get; }

    public ImageSharpFileFormat(string name, string[] extensions)
    {
        Name = name;
        Extensions = extensions;
    }

    public override void Save(ViewportBitmap bitmap, string filename)
    {
        using var img = new Image<Rgba32>(bitmap.Width, bitmap.Height);

        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                PwColor color = bitmap.Get(x, y);
                img[x, y] = new Rgba32(color.R, color.G, color.B, color.A);
            }
        }
        
        SaveInternal(img, filename);
    }

    protected abstract void SaveInternal(Image<Rgba32> image, string filename);

    public override ViewportBitmap? Load(string filename)
    {
        using var img = Image.Load<Rgba32>(filename);
        ViewportBitmap bitmap = new ViewportBitmap(img.Width, img.Height);

        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                Rgba32 color = img[x, y];
                bitmap.Set(x, y, new PwColor(color.R, color.G, color.B, color.A));
            }
        }
        
        return bitmap;
    }
}