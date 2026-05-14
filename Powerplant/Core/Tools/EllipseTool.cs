using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using Powerplant.Core.Commands;
using Powerplant.Core.UndoRedo;

namespace Powerplant.Core.Tools;

public class EllipseTool : RectangleBaseTool
{
    public EllipseTool()
    {
        
    }
    
    HashSet<(int x, int y)> GenerateEllipse(int x, int y, int width, int height)
    {
        HashSet<(int x, int y)> set = [];
        
        double rx = width / 2.0;
        double ry = height / 2.0;

        double centerX = x + rx;
        double centerY = y + ry;

        for (int py = y; py < y + height; py++)
        {
            for (int px = x; px < x + width; px++)
            {
                double dx = (px + 0.5 - centerX) / rx;
                double dy = (py + 0.5 - centerY) / ry;

                double distance = dx * dx + dy * dy;

                if (distance <= 1.0) set.Add((px, py));
            }
        }

        return set;
    }
    
    protected override void Apply(int x, int y, int width, int height)
    {
        HashSet<(int x, int y)> ellipse = GenerateEllipse(x, y, width, height);
        Viewport.RunCommand(new PixelsCommand(ellipse, Viewport.PrimaryColor));
    }

    protected override void RenderPreview(DrawingContext context, Rect previewRect)
    {
        context.DrawEllipse(Viewport.PrimaryColorBrush, null, previewRect);
    }
}