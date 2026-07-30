using System.Collections.Generic;
using System.Numerics;

namespace Powerplant.Core.Commands;

public class MovePixelsCommand : Command
{
    private ViewportBitmap _oldBitmap;
    private Vector2[] _pixels;
    private Vector2 _delta;
    private PixelSelection _oldSelection;
        
    public MovePixelsCommand(Vector2[] pixels, Vector2 delta)
    {
        _pixels = pixels;
        _delta = delta;
    }

    public override void Init()
    {
        _oldSelection = Viewport.Selection;
        _oldBitmap = Bitmap.Copy();
    }

    public override void Run()
    {
        Dictionary<Vector2, PwColor> colors = [];

        foreach (Vector2 pixel in _pixels)
        {
            colors.Add(pixel, Bitmap.Get((int)pixel.X, (int)pixel.Y));
            Bitmap.Set((int)pixel.X, (int)pixel.Y, PwColor.Transparent);
        }
            
        foreach (KeyValuePair<Vector2, PwColor> kv in colors)
            Bitmap.Set((int)(kv.Key.X + _delta.X), (int)(kv.Key.Y + _delta.Y), kv.Value);

        PixelSelection offsetSelection = Viewport.Selection.Copy();
        offsetSelection.Offset(_delta);
        Viewport.SetSelection(offsetSelection);
    }

    public override void Undo()
    {
        Viewport.SetBitmap(_oldBitmap);
        Viewport.SetSelection(_oldSelection);
    }
}