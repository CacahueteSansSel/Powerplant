namespace Powerplant.Core.Tools;

public class EraserTool : ViewportTool
{
    public override void UsePrimary(int cursorX, int cursorY)
    {
        Bitmap.Set(cursorX, cursorY, PwColor.Transparent);
    }

    public override void UseSecondary(int cursorX, int cursorY)
    {
        Bitmap.Set(cursorX, cursorY, PwColor.Transparent);
    }
}