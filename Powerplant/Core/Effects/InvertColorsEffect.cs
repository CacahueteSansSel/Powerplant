using ExCSS;

namespace Powerplant.Core.Effects;

public class InvertColorsEffect : PixelEffect
{
    public override PwColor Process(int x, int y, ViewportBitmap referenceBitmap, ViewportBitmap targetBitmap, PixelSelection area)
    {
        PwColor originalColor = referenceBitmap.Get(x, y);
        if (originalColor.A == 0) return originalColor;

        return new PwColor((byte)(255 - originalColor.R), (byte)(255 - originalColor.G), 
            (byte)(255 - originalColor.B), originalColor.A);
    }
}