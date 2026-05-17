using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Powerplant.Controls;
using Powerplant.Controls.ToolsSettings;

namespace Powerplant.Core.Tools;

public abstract class ViewportTool
{
    public abstract string Name { get; }
    public virtual bool SupportsHold => true;
    public virtual Control? ToolSettingsControl => null;
    
    public ViewportControl Viewport { get; internal set; }
    
    protected ViewportBitmap Bitmap => Viewport.Bitmap;

    public abstract void UsePrimary(int cursorX, int cursorY);
    public abstract void UseSecondary(int cursorX, int cursorY);

    public virtual void OnPointerDown(int cursorX, int cursorY) {}
    public virtual void OnPointerUp(int cursorX, int cursorY) {}
    public virtual void OnPointerMove(int cursorX, int cursorY) {}

    public virtual void Render(DrawingContext context) {}
}