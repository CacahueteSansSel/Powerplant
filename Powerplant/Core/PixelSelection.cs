using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Powerplant.Core;

public class PixelSelection
{
    public static PixelSelection Empty => new([]);
    
    public static PixelSelection Rectangle(int x, int y, int width, int height)
    {
        List<Vector2> pixels = [];

        for (int py = y; py < y + height; py++)
        {
            for (int px = x; px < x + width; px++)
            {
                pixels.Add(new Vector2(px, py));
            }
        }

        return new PixelSelection(pixels.ToArray());
    }

    public static PixelSelection List(params Vector2[] pixels) 
        => new(pixels);
    
    private List<Vector2> _pixels = [];

    public Vector2[] Pixels => _pixels.ToArray();
    public bool IsEmpty => Pixels.Length == 0;

    private PixelSelection()
    {
        
    }

    private PixelSelection(Vector2[] pixels)
    {
        _pixels = new List<Vector2>(pixels);
    }

    public void Add(Vector2 pixel)
    {
        if (_pixels.Contains(pixel)) return;
        
        _pixels.Add(pixel);
    }

    public void Remove(Vector2 pixel)
    {
        if (!_pixels.Contains(pixel)) return;
        
        _pixels.Remove(pixel);
    }

    public bool Contains(Vector2 position) 
        => _pixels.Any(pos => pos == position);

    public bool Contains(int x, int y) 
        => _pixels.Any(pos => (int)pos.X == x && (int)pos.Y == y);
}