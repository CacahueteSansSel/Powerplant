using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Avalonia.Media.Imaging;
using Powerplant.Core.Commands;
using Powerplant.Core.UndoRedo;

namespace Powerplant.Core.Tools;

public class MagicWandTool : ViewportTool
{
    public override void UsePrimary(int cursorX, int cursorY)
    {
        MagicWand(cursorX, cursorY, Bitmap.Get(cursorX, cursorY));
    }

    public override void UseSecondary(int cursorX, int cursorY)
    {
        MagicWand(cursorX, cursorY, Bitmap.Get(cursorX, cursorY));
    }

    void MagicWand(int startX, int startY, PwColor target)
    {
        Stack<(int x, int y)> stack = new Stack<(int x, int y)>();
        stack.Push((startX, startY));

        HashSet<(int x, int y)> visited = new HashSet<(int x, int y)>();
        HashSet<(int x, int y)> finalPixels = new HashSet<(int x, int y)>();

        while (stack.Count > 0)
        {
            var (x, y) = stack.Pop();

            if (x < 0 || y < 0 || x >= Bitmap.Width || y >= Bitmap.Height)
                continue;
            if (!Viewport.Selection.IsEmpty && !Viewport.Selection.Contains(x, y))
                continue;

            if (visited.Contains((x, y)))
                continue;

            visited.Add((x, y));

            if (Bitmap.Get(x, y) != target)
                continue;

            finalPixels.Add((x, y));

            stack.Push((x - 1, y));
            stack.Push((x + 1, y));
            stack.Push((x, y - 1));
            stack.Push((x, y + 1));
        }
        
        Viewport.RunCommand(new SelectionCommand(PixelSelection.List(finalPixels.Select(tuple => new Vector2(tuple.x, tuple.y)).ToArray())));
    }
}