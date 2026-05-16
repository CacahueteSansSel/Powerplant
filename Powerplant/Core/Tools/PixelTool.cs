using Powerplant.Core.Commands;
using Powerplant.Core.UndoRedo;

namespace Powerplant.Core.Tools;

public class PixelTool : ViewportTool
{
    public override void UsePrimary(int cursorX, int cursorY)
    {
        if (Bitmap.Get(cursorX, cursorY) == Viewport.PrimaryColor)
            return;
        if (!Viewport.Selection.IsEmpty && !Viewport.Selection.Contains(cursorX, cursorY))
            return;
        
        Viewport.RunCommand(new PixelToolCommand(cursorX, cursorY, Bitmap.Get(cursorX, cursorY), Viewport.PrimaryColor));
    }

    public override void UseSecondary(int cursorX, int cursorY)
    {
        if (Bitmap.Get(cursorX, cursorY) == Viewport.SecondaryColor)
            return;
        if (!Viewport.Selection.IsEmpty && !Viewport.Selection.Contains(cursorX, cursorY))
            return;
        
        Viewport.RunCommand(new PixelToolCommand(cursorX, cursorY, Bitmap.Get(cursorX, cursorY), Viewport.SecondaryColor));
    }

    class PixelToolCommand : Command
    {
        private int _x, _y;
        private PwColor _oldColor;
        private PwColor _newColor;

        public PixelToolCommand(int x, int y, PwColor oldColor, PwColor newColor)
        {
            _x = x;
            _y = y;
            _oldColor = oldColor;
            _newColor = newColor;
        }

        public override void Run()
        {
            Bitmap.Set(_x, _y, _newColor);
        }

        public override void Undo()
        {
            Bitmap.Set(_x, _y, _oldColor);
        }
    }
}