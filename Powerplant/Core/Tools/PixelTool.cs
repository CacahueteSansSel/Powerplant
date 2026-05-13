namespace Powerplant.Core.Tools;

public class PixelTool : ViewportTool
{
    public override void UsePrimary(int cursorX, int cursorY)
    {
        Bitmap.Set(cursorX, cursorY, Viewport.PrimaryColor);
    }

    public override void UseSecondary(int cursorX, int cursorY)
    {
        Bitmap.Set(cursorX, cursorY, Viewport.SecondaryColor);
    }
}