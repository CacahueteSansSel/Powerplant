using System;
using Avalonia;
using Avalonia.Media;
using Powerplant.Core.Commands;
using Powerplant.Core.UndoRedo;

namespace Powerplant.Core.Tools;

public class SelectionRectangleTool : RectangleBaseTool
{
    public override string Name => "Rectangle Selection";
    
    public SelectionRectangleTool()
    {
        
    }
    
    protected override void Apply(int x, int y, int width, int height)
    {
        Viewport.RunCommand(new SelectionCommand(PixelSelection.Rectangle(x, y, width, height)));
    }

    protected override void RenderPreview(DrawingContext context, Rect previewRect)
    {
        context.DrawRectangle(Viewport.SelectionBrush, Viewport.SelectionPen, previewRect);
    }
}