using Powerplant.Core.Commands;
using Powerplant.Core.UndoRedo;

namespace Powerplant.Core.Tools;

public class EraserTool : ViewportTool
{
    public override void UsePrimary(int cursorX, int cursorY)
    {
        Viewport.RunCommand(new EraserToolCommand(cursorX, cursorY, Bitmap.Get(cursorX, cursorY)));
    }

    public override void UseSecondary(int cursorX, int cursorY)
    {
        Viewport.RunCommand(new EraserToolCommand(cursorX, cursorY, Bitmap.Get(cursorX, cursorY)));
    }

    class EraserToolCommand : Command
    {
        private int _x, _y;
        private PwColor _oldColor;

        public EraserToolCommand(int x, int y, PwColor oldColor)
        {
            _x = x;
            _y = y;
            _oldColor = oldColor;
        }

        public override void Run()
        {
            Bitmap.Set(_x, _y, PwColor.Transparent);
        }

        public override void Undo()
        {
            Bitmap.Set(_x, _y, _oldColor);
        }
    }
}