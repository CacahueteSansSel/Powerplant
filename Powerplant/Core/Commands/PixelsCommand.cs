using System.Collections.Generic;

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