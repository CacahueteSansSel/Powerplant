using System.Collections.Generic;

namespace Powerplant.Core.Tools;

public class FloodFillTool : ViewportTool
{
    public override void UsePrimary(int cursorX, int cursorY)
    {
        FloodFill(cursorX, cursorY, Bitmap.Get(cursorX, cursorY), Viewport.PrimaryColor);
    }

    public override void UseSecondary(int cursorX, int cursorY)
    {
        FloodFill(cursorX, cursorY, Bitmap.Get(cursorX, cursorY), Viewport.SecondaryColor);
    }

    void FloodFill(int startX, int startY, PwColor target, PwColor replacement)
    {
        if (target == replacement)
            return;

        var stack = new Stack<(int x, int y)>();
        stack.Push((startX, startY));

        while (stack.Count > 0)
        {
            var (x, y) = stack.Pop();

            if (x < 0 || y < 0 || x >= Bitmap.Width || y >= Bitmap.Height)
                continue;

            if (Bitmap.Get(x, y) != target)
                continue;

            Bitmap.Set(x, y, replacement);

            stack.Push((x - 1, y));
            stack.Push((x + 1, y));
            stack.Push((x, y - 1));
            stack.Push((x, y + 1));
        }
    }
}