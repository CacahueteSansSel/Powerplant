using System;
using Avalonia;
using Avalonia.Media;
using Powerplant.Core.Commands;
using Powerplant.Core.UndoRedo;

namespace Powerplant.Core.Tools;

public class RectangleTool : RectangleBaseTool
{
    public RectangleTool()
    {
        
    }
    
    protected override void Apply(int x, int y, int width, int height)
    {
        Viewport.RunCommand(new RectangleCommand(x, y, width, height, Viewport.PrimaryColor));
    }

    protected override void RenderPreview(DrawingContext context, Rect previewRect)
    {
        context.FillRectangle(Viewport.PrimaryColorBrush, previewRect);
    }
    
    public class RectangleCommand : Command
    {
        private int _x, _y, _width, _height;
        private ViewportBitmap _oldData;
        private PwColor _color;

        public RectangleCommand(int x, int y, int width, int height, PwColor color)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
            _color = color;
        }

        public override void Init()
        {
            _oldData = Bitmap.Copy(_x, _y, _width, _height);
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