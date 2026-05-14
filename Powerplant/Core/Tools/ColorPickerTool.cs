using Powerplant.Core.UndoRedo;

namespace Powerplant.Core.Tools;

public class ColorPickerTool : ViewportTool
{
    public override void UsePrimary(int cursorX, int cursorY)
    {
        Viewport.RunCommand(new ColorPickerCommand(Viewport.PrimaryColor, Bitmap.Get(cursorX, cursorY), false));
    }

    public override void UseSecondary(int cursorX, int cursorY)
    {
        Viewport.RunCommand(new ColorPickerCommand(Viewport.SecondaryColor, Bitmap.Get(cursorX, cursorY), true));
    }

    class ColorPickerCommand : Command
    {
        private PwColor _oldColor;
        private PwColor _newColor;
        private bool _isSecondary;

        public ColorPickerCommand(PwColor oldColor, PwColor newColor, bool isSecondary)
        {
            _oldColor = oldColor;
            _newColor = newColor;
            _isSecondary = isSecondary;
        }

        public override void Run()
        {
            if (_isSecondary) Viewport.SetSecondaryColor(_newColor);
            else Viewport.SetPrimaryColor(_newColor);
        }

        public override void Undo()
        {
            if (_isSecondary) Viewport.SetSecondaryColor(_oldColor);
            else Viewport.SetPrimaryColor(_oldColor);
        }
    }
}