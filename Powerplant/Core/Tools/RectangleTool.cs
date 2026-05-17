using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Powerplant.Controls.ToolsSettings;
using Powerplant.Core.Commands;
using Powerplant.Core.UndoRedo;

namespace Powerplant.Core.Tools;

public class RectangleTool : RectangleBaseTool
{
    public override string Name => "Draw Rectangle";
    public ToolSettings Settings { get; } = new();
    public override Control? ToolSettingsControl => new DrawRectangleToolSettings(this);

    public RectangleTool()
    {
        
    }
    
    protected override void Apply(int x, int y, int width, int height)
    {
        Viewport.RunCommand(new RectangleCommand(x, y, width, height, Viewport.PrimaryColor, !Settings.IsFilled, Settings.Thickness));
    }

    protected override void RenderPreview(DrawingContext context, Rect previewRect)
    {
        if (Settings.IsFilled)
        {
            context.FillRectangle(Viewport.PrimaryColorBrush, previewRect);
            return;
        }

        Pen pen = new Pen(Viewport.PrimaryColor.ToBgra(), Settings.Thickness * Viewport.Zoom);
        context.DrawRectangle(pen, new Rect(previewRect.X + Viewport.Zoom / 2f, previewRect.Y + Viewport.Zoom / 2f, 
            previewRect.Width-Viewport.Zoom, previewRect.Height-Viewport.Zoom));
    }

    public class ToolSettings
    {
        public bool IsFilled { get; set; }
        public int Thickness { get; set; }

        public ToolSettings()
        {
            IsFilled = true;
            Thickness = 1;
        }
    }
    
    public class RectangleCommand : Command
    {
        private int _x, _y, _width, _height;
        private ViewportBitmap _oldData;
        private PwColor _color;
        private bool _outline;
        private int _outlineSize;

        public RectangleCommand(int x, int y, int width, int height, PwColor color, bool isOutline, int outlineSize)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
            _color = color;

            _outline = isOutline;
            _outlineSize = outlineSize;
        }

        public override void Init()
        {
            _oldData = Bitmap.Copy(_x, _y, _width, _height);
        }

        public override void Run()
        {
            if (_outline)
            {
                for (int ry = _y; ry < _y + _height; ry++)
                {
                    for (int rx = _x; rx < _x + _width; rx++)
                    {
                        if (!Viewport.Selection.IsEmpty && !Viewport.Selection.Contains(rx, ry)) continue;
                        if (rx >= _x + _outlineSize && rx < _x + _width - _outlineSize &&
                            ry >= _y + _outlineSize && ry < _y + _height - _outlineSize) continue;
                        
                        Bitmap.Set(rx, ry, _color);
                    }
                }
                
                return;
            }
            
            for (int ry = _y; ry < _y + _height; ry++)
            {
                for (int rx = _x; rx < _x + _width; rx++)
                {
                    if (!Viewport.Selection.IsEmpty && !Viewport.Selection.Contains(rx, ry))
                        continue;
                    
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