using Avalonia.Input;
using Powerplant.Core.Commands;
using Powerplant.Core.UndoRedo;
using Powerplant.Utilities;

namespace Powerplant.Core.Tools;

public class MulticolorPixelTool : ViewportTool
{
    public override string Name => "Multicolor Pencil";
    public override Cursor? Cursor => PwCursors.Pencil;
    private int _counter = 0;

    public override void UsePrimary(int cursorX, int cursorY)
    {
        if (Bitmap.Get(cursorX, cursorY) == Viewport.PrimaryColor)
            return;
        if (!Viewport.Selection.IsEmpty && !Viewport.Selection.Contains(cursorX, cursorY))
            return;

        PwColor color = PwColor.FromHsv(_counter, 1f, 1f);
        _counter++;
        if (_counter >= 360) _counter = 0;

        Viewport.RunCommand(new PixelToolCommand(cursorX, cursorY, Bitmap.Get(cursorX, cursorY), color));
    }

    public override void UseSecondary(int cursorX, int cursorY)
        => UsePrimary(cursorX, cursorY);

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
            Bitmap.Set(_x, _y, _newColor, true);
        }

        public override void Undo()
        {
            Bitmap.Set(_x, _y, _oldColor, false);
        }
    }
}