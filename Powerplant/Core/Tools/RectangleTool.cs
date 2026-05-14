using System;
using Avalonia;
using Avalonia.Media;
using Powerplant.Core.UndoRedo;

namespace Powerplant.Core.Tools;

public class RectangleTool : ViewportTool
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
        _isDrawing = false;
        
        ApplyRectangle(_x, _y, _width, _height);
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

    private void ApplyRectangle(int x, int y, int width, int height)
    {
        Viewport.RunCommand(new RectangleCommand(x, y, width, height, 
            Bitmap.Copy(x, y, width, height), Viewport.PrimaryColor));
    }

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

        context.FillRectangle(Viewport.PrimaryColorBrush, previewRect);
    }

    class RectangleCommand : Command
    {
        private int _x, _y, _width, _height;
        private ViewportBitmap _oldData;
        private PwColor _color;

        public RectangleCommand(int x, int y, int width, int height, ViewportBitmap oldData, PwColor color)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
            _oldData = oldData;
            _color = color;
        }

        public override void Run()
        {
            for (int ry = _y; ry < _y + _height; ry++)
            {
                for (int rx = _x; rx < _x + _width; rx++)
                {
                    Bitmap.Set(rx, ry, _color);
                }
            }
        }

        public override void Undo()
        {
            Bitmap.ApplyBitmap(_oldData, _x, _y);
        }
    }
}