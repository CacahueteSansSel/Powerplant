using System;
using Powerplant.Utilities;

namespace Powerplant.Core.Effects;

public class SepiaEffect : PixelEffect
{
    public override PwColor Process(int x, int y, ViewportBitmap referenceBitmap, ViewportBitmap targetBitmap, PixelSelection area)
    {
        PwColor refColor = referenceBitmap.Get(x, y);

        return new PwColor(
            MathUtilities.ClampByte(0.393f * refColor.R + 0.769f * refColor.G + 0.189f * refColor.B),
            MathUtilities.ClampByte(0.349f * refColor.R + 0.686f * refColor.G + 0.168f * refColor.B),
            MathUtilities.ClampByte(0.272f * refColor.R + 0.534f * refColor.G + 0.131f * refColor.B),
            refColor.A
        );
    }
}