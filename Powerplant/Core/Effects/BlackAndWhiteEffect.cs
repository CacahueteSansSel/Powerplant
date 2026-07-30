using System;

namespace Powerplant.Core.Effects;

public class BlackAndWhiteEffect : PixelEffect
{
    public override PwColor Process(int x, int y, ViewportBitmap referenceBitmap, ViewportBitmap targetBitmap, PixelSelection area)
    {
        PwColor refColor = referenceBitmap.Get(x, y);

        byte gray = (byte)(0.2126f * refColor.R + 0.7152f * refColor.G + 0.0722f * refColor.B);

        return new PwColor(gray, gray, gray, refColor.A);
    }
}