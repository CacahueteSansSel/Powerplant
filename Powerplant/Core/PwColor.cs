using Avalonia.Media;

namespace Powerplant.Core;

public struct PwColor
{
    public static PwColor Transparent => new(0, 0, 0, 0);
    public static PwColor Black => new(0, 0, 0);
    public static PwColor White => new(255, 255, 255);
    public static PwColor Red => new(255, 0, 0);
    public static PwColor Green => new(0, 255, 0);
    public static PwColor Blue => new(0, 0, 255);
    
    public byte R;
    public byte G;
    public byte B;
    public byte A;

    public PwColor(Color color) : this(color.R, color.G, color.B, color.A)
    {
        
    }

    public PwColor(byte r, byte g, byte b, byte a = byte.MaxValue)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public uint ToBgra()
        => (uint)(B | (G << 8) | (R << 16) | (A << 24));

    public Color ToColor()
        => new(A, R, G, B);
}