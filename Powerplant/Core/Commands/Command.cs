using Powerplant.Controls;

namespace Powerplant.Core.Commands;

public abstract class Command
{
    public ViewportControl Viewport { get; internal set; }
    public ViewportBitmap Bitmap => Viewport.Bitmap;

    public virtual void Init() {}
    
    public abstract void Run();
    public abstract void Undo();
}