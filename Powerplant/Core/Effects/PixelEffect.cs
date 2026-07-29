using System.Numerics;

namespace Powerplant.Core.Effects;

public abstract class PixelEffect : Effect
{
    public override bool Apply(ViewportBitmap referenceBitmap, ViewportBitmap targetBitmap, PixelSelection area)
    {
        if (!area.IsEmpty)
        {
            foreach (Vector2 px in area.Pixels)
            {
                targetBitmap.Set((int)px.X, (int)px.Y, 
                    Process((int)px.X, (int)px.Y, referenceBitmap, targetBitmap, area));
            }

            return true;
        }
        
        for (int y = 0; y < referenceBitmap.Height; y++)
        {
            for (int x = 0; x < referenceBitmap.Width; x++)
            {
                targetBitmap.Set(x, y, Process(x, y, referenceBitmap, targetBitmap, area));
            }
        }

        return true;
    }

    public abstract PwColor Process(int x, int y, ViewportBitmap referenceBitmap, ViewportBitmap targetBitmap, PixelSelection area);
}