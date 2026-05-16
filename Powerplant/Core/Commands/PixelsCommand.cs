using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Powerplant.Core.Commands;

public class PixelsCommand : Command
{
    private HashSet<(int x, int y)> _pixelList;
    private PwColor _newColor;
    private ViewportBitmap _oldBitmap;

    public PixelsCommand(HashSet<(int x, int y)> pixelList, PwColor newColor)
    {
        _pixelList = pixelList;
        _newColor = newColor;
    }

    public PixelsCommand(Vector2[] pixels, PwColor newColor)
    {
        _pixelList = new HashSet<(int x, int y)>(pixels.Select(p => ((int)p.X, (int)p.Y)));
        _newColor = newColor;
    }

    public override void Init()
    {
        _oldBitmap = Bitmap.Copy();
    }

    public override void Run()
    {
        foreach (var pixel in _pixelList)
            Bitmap.Set(pixel.x, pixel.y, _newColor);
    }

    public override void Undo()
    {
        Bitmap.ApplyBitmap(_oldBitmap);
    }
}