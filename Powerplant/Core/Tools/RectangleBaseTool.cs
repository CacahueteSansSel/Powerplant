using Avalonia;
using Avalonia.Media;

namespace Powerplant.Core.Tools;

public abstract class RectangleBaseTool : ViewportTool
{
    private int _initialCursorX;
    private int _initialCursorY;
    private int _x;
    private int _y;
    private int _width;
    private int _height;
    private bool _isDrawing;
    
    public override void UsePrimary(int cursorX, int cursorY)
    {
        
    }

    public override void UseSecondary(int cursorX, int cursorY)
    {
        
    }

    public override void OnPointerDown(int cursorX, int cursorY)
    {
        _isDrawing = true;
        
        _initialCursorX = cursorX;
        _initialCursorY = cursorY;
    }

    public override void OnPointerUp(int cursorX, int cursorY)
    {
        if (!_isDrawing) return;
        _isDrawing = false;
        
        Apply(_x, _y, _width, _height);

        _x = 0;
        _y = 0;
        _width = 0;
        _height = 0;
    }
    
    Rect NormalizeRect(int x, int y, int width, int height)
    {
        if (width < 0)
        {
            x += width;
            width = -width;
        }
        
        if (height < 0)
        {
            y += height;
            height = -height;
        }

        return new Rect(x, y, width, height);
    }

    protected abstract void Apply(int x, int y, int width, int height);

    public override void OnPointerMove(int cursorX, int cursorY)
    {
        if (!_isDrawing) return;
        
        Rect normRect = NormalizeRect(_initialCursorX, _initialCursorY, 
            cursorX - _initialCursorX, cursorY - _initialCursorY);

        _x = (int)normRect.X;
        _y = (int)normRect.Y;
        _width = (int)normRect.Width+1;
        _height = (int)normRect.Height+1;

        Viewport.InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        if (!_isDrawing) return;
        
        // Render the rectangle's preview

        Rect previewRect = new Rect(
            Viewport.InvertTransformCoordX(_x),
            Viewport.InvertTransformCoordY(_y),
            Viewport.InvertTransformX(_width),
            Viewport.InvertTransformY(_height)
        );

        using DrawingContext.PushedState state = context.PushOpacity(0.5);
        RenderPreview(context, previewRect);
    }

    protected abstract void RenderPreview(DrawingContext context, Rect previewRect);
}