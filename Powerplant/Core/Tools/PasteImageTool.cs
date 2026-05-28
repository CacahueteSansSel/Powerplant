using System.Numerics;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Powerplant.Core.Commands;

namespace Powerplant.Core.Tools;

public class PasteImageTool : ViewportTool
{
    private Bitmap _bitmap;
    private Vector2? _position;
    
    public override string Name => "Paste image";

    public PasteImageTool(Bitmap bitmap)
    {
        _bitmap = bitmap;
    }
    
    public override void UsePrimary(int cursorX, int cursorY)
    {
        Viewport.RunCommand(new BitmapCommand(cursorX, cursorY, _bitmap));
    }

    public override void UseSecondary(int cursorX, int cursorY)
    {
        
    }

    public override void OnPointerMove(int cursorX, int cursorY)
    {
        _position = new Vector2(cursorX, cursorY);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        
        if (_position == null) return;

        using DrawingContext.PushedState state = context.PushOpacity(0.5f);

        Rect bounds = new(Viewport.InvertTransformCoordX(_position.Value.X), 
            Viewport.InvertTransformCoordY(_position.Value.Y),
            Viewport.InvertTransformX(_bitmap.PixelSize.Width), 
            Viewport.InvertTransformY(_bitmap.PixelSize.Height));
        
        context.DrawImage(_bitmap, bounds);
    }
}