namespace Powerplant.Core.Effects;

public abstract class PixelEffect : Effect
{
    public override bool Apply(ViewportBitmap referenceBitmap, ViewportBitmap targetBitmap)
    {
        for (int y = 0; y < referenceBitmap.Height; y++)
        {
            for (int x = 0; x < referenceBitmap.Width; x++)
            {
                targetBitmap.Set(x, y, Process(x, y, referenceBitmap, targetBitmap));
            }
        }

        return true;
    }

    public abstract PwColor Process(int x, int y, ViewportBitmap referenceBitmap, ViewportBitmap targetBitmap);
}