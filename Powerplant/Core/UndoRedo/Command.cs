using Powerplant.Controls;

namespace Powerplant.Core.UndoRedo;

public abstract class Command
{
    public ViewportControl Viewport { get; internal set; }
    public ViewportBitmap Bitmap => Viewport.Bitmap;
    
    public abstract void Run();
    public abstract void Undo();
}