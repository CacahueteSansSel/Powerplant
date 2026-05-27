using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Powerplant.Core.Commands;
using Powerplant.Core.UndoRedo;
using SelectionMode = Powerplant.Core.Commands.SelectionMode;

namespace Powerplant.Core.Tools;

public class SelectionRectangleTool : RectangleBaseTool
{
    public override string Name => "Rectangle Selection";
    public override Key? Key => Avalonia.Input.Key.S;

    public SelectionRectangleTool()
    {
        
    }
    
    protected override void Apply(int x, int y, int width, int height)
    {
        SelectionMode mode = SelectionMode.Set;

        if (Viewport.IsShiftPressed)
            mode = SelectionMode.Add;
        
        Viewport.RunCommand(new SelectionCommand(PixelSelection.Rectangle(x, y, width, height), mode));
    }

    protected override void RenderPreview(DrawingContext context, Rect previewRect)
    {
        context.DrawRectangle(Viewport.SelectionBrush, Viewport.SelectionPen, previewRect);
    }
}