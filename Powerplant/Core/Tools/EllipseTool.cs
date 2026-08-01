using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Powerplant.Controls.ToolsSettings;
using Powerplant.Core.Commands;
using Powerplant.Core.UndoRedo;

namespace Powerplant.Core.Tools;

public class EllipseTool : RectangleBaseTool
{
    public override string Name => "Draw Ellipse";

    public ToolSettings Settings { get; } = new();
    public override Control? ToolSettingsControl => new DrawEllipseToolSettings(this);
    public override Key? Key => Avalonia.Input.Key.L;
    
    public EllipseTool()
    {
        
    }
    
    HashSet<(int x, int y)> GenerateEllipse(int x, int y, int width, int height, bool outline, int thickness)
    {
        HashSet<(int x, int y)> set = [];
        
        double rx = width / 2.0;
        double ry = height / 2.0;

        double centerX = x + rx;
        double centerY = y + ry;

        double innerRx = Math.Max(0.0, rx - thickness);
        double innerRy = Math.Max(0.0, ry - thickness);

        for (int py = y; py < y + height; py++)
        {
            for (int px = x; px < x + width; px++)
            {
                double dx = px + 0.5 - centerX;
                double dy = py + 0.5 - centerY;

                double outer =
                    (dx * dx) / (rx * rx) +
                    (dy * dy) / (ry * ry);

                bool isPixel;

                if (outline)
                {
                    double inner =
                        (dx * dx) / (innerRx * innerRx) +
                        (dy * dy) / (innerRy * innerRy);

                    isPixel = outer <= 1.0 && inner >= 1.0;
                }
                else
                {
                    isPixel = outer <= 1.0;
                }

                if (isPixel)
                {
                    if (!Viewport.Selection.IsEmpty &&
                        !Viewport.Selection.Contains(px, py))
                        continue;

                    set.Add((px, py));
                }
            }
        }

        return set;
    }
    
    protected override void Apply(int x, int y, int width, int height)
    {
        HashSet<(int x, int y)> ellipse = GenerateEllipse(x, y, width, height, !Settings.IsFilled, Settings.Thickness);
        Viewport.RunCommand(new PixelsCommand(ellipse, Viewport.PrimaryColor, true));
    }

    protected override void RenderPreview(DrawingContext context, Rect previewRect)
    {
        context.DrawEllipse(Viewport.PrimaryColorBrush, null, previewRect);
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
}