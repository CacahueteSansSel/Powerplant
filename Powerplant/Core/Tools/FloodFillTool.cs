namespace Powerplant.Core.Tools;

public class FloodFillTool : ViewportTool
{
    public override void UsePrimary(int cursorX, int cursorY)
    {
        FloodFill(cursorX, cursorY, Bitmap.Get(cursorX, cursorY), Viewport.PrimaryColor);
    }

    public override void UseSecondary(int cursorX, int cursorY)
    {
        FloodFill(cursorX, cursorY, Bitmap.Get(cursorX, cursorY), Viewport.SecondaryColor);
    }

    void FloodFill(int x, int y, PwColor toReplaceColor, PwColor finalColor)
    {
        if (x < 0 || y < 0 || x >= Bitmap.Width || y >= Bitmap.Height)
            return;
        if (Bitmap.Get(x, y) != toReplaceColor || Bitmap.Get(x, y) == finalColor)
            return;
        
        Bitmap.Set(x, y, finalColor);
        
        // The famous infinite shit machine
        FloodFill(x - 1, y, toReplaceColor, finalColor);
        FloodFill(x + 1, y, toReplaceColor, finalColor);
        FloodFill(x, y - 1, toReplaceColor, finalColor);
        FloodFill(x, y + 1, toReplaceColor, finalColor);
    }
}