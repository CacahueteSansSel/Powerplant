namespace Powerplant.Core.Effects;

public class PureBlackEffect : PixelEffect
{
    public override PwColor Process(int x, int y, ViewportBitmap referenceBitmap, ViewportBitmap targetBitmap)
    {
        return referenceBitmap.Get(x, y) with { R = 0, G = 0, B = 0 };
    }
}