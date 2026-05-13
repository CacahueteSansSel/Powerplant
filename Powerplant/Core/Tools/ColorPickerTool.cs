namespace Powerplant.Core.Tools;

public class ColorPickerTool : ViewportTool
{
    public override void UsePrimary(int cursorX, int cursorY)
    {
        Viewport.SetPrimaryColor(Bitmap.Get(cursorX, cursorY));
    }

    public override void UseSecondary(int cursorX, int cursorY)
    {
        Viewport.SetSecondaryColor(Bitmap.Get(cursorX, cursorY));
    }
}