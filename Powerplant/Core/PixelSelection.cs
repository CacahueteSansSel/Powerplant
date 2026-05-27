using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Avalonia;
using Vector = System.Numerics.Vector;

namespace Powerplant.Core;

public class PixelSelection
{
    public static PixelSelection Empty => new(Array.Empty<Vector2>());
    
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
    public Rect Bounds { get; private set; }

    private PixelSelection()
    {
        
    }

    private PixelSelection(Vector2[] pixels)
    {
        _pixels = new List<Vector2>(pixels);

        UpdateRect();
    }

    private PixelSelection(List<Vector2> pixels)
    {
        _pixels = pixels;
    }

    private void UpdateRect()
    {
        Vector2 min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        Vector2 max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

        foreach (Vector2 pos in _pixels)
        {
            if (pos.X < min.X) min.X = pos.X;
            if (pos.Y < min.Y) min.Y = pos.Y;

            if (pos.X > max.X) max.X = pos.X;
            if (pos.Y > max.Y) max.Y = pos.Y;
        }

        Bounds = new Rect(new Point(min.X, min.Y), new Point(max.X+1, max.Y+1));
    }

    public PixelSelection Copy()
        => new(_pixels);

    public void Add(Vector2 pixel)
    {
        if (_pixels.Contains(pixel)) return;
        
        _pixels.Add(pixel);
        UpdateRect();
    }

    public void Add(Vector2[] pixels)
    {
        foreach (Vector2 pixel in pixels)
        {
            if (!_pixels.Contains(pixel))
                _pixels.Add(pixel);
        }
        
        UpdateRect();
    }

    public void Add(PixelSelection selection)
        => Add(selection.Pixels);

    public void Remove(Vector2 pixel)
    {
        if (!_pixels.Contains(pixel)) return;
        
        _pixels.Remove(pixel);
        UpdateRect();
    }

    public bool Contains(Vector2 position) 
        => _pixels.Any(pos => pos == position);

    public bool Contains(int x, int y) 
        => _pixels.Any(pos => (int)pos.X == x && (int)pos.Y == y);

    public void Offset(Vector2 offset)
    {
        _pixels = _pixels.Select(p => p + offset).ToList();
        UpdateRect();
    }
}