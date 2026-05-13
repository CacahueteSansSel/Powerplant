using System;
using Avalonia.Media;

namespace Powerplant.Core.Tools;

public class DebugTool : ViewportTool
{
    public override void UsePrimary(int cursorX, int cursorY)
    {
        Console.WriteLine($"UsePrimary {cursorX} {cursorY}");
    }

    public override void UseSecondary(int cursorX, int cursorY)
    {
        Console.WriteLine($"UseSecondary {cursorX} {cursorY}");
    }

    public override void OnPointerDown(int cursorX, int cursorY)
    {
        Console.WriteLine($"OnPointerDown {cursorX} {cursorY}");
    }

    public override void OnPointerUp(int cursorX, int cursorY)
    {
        Console.WriteLine($"OnPointerUp {cursorX} {cursorY}");
    }

    public override void OnPointerMove(int cursorX, int cursorY)
    {
        Console.WriteLine($"OnPointerMove {cursorX} {cursorY}");
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
    }
}