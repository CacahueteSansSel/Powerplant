using Avalonia;

namespace Powerplant.Core.Effects;

public class RotateM90Effect : Effect
{
    public override bool Apply(ViewportBitmap referenceBitmap, ViewportBitmap targetBitmap, PixelSelection area)
    {
        Rect bounds = area.IsEmpty ? new Rect(0, 0, referenceBitmap.Width, referenceBitmap.Height) : area.Bounds;
        
        for (int y = (int)bounds.Y; y < bounds.Y + bounds.Height; y++)
        {
            for (int x = (int)bounds.X; x < bounds.X + bounds.Width; x++)
            {
                if (!area.IsEmpty && !area.Contains(x, y)) continue;
                
                PwColor refColor = referenceBitmap.Get(x, y);
                targetBitmap.Set(y, referenceBitmap.Width - 1 - x, refColor);
            }
        }

        return true;
    }
}