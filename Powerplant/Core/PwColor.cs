using System;
using System.Globalization;
using Avalonia.Media;
using SkiaSharp;

namespace Powerplant.Core;

public struct PwColor : IEquatable<PwColor>
{
    public static PwColor Transparent => new(0, 0, 0, 0);
    public static PwColor Black => new(0, 0, 0);
    public static PwColor White => new(255, 255, 255);
    public static PwColor Red => new(255, 0, 0);
    public static PwColor Green => new(0, 255, 0);
    public static PwColor Blue => new(0, 0, 255);

    public static PwColor AlphaPremultiply(PwColor backgroundColor, PwColor color)
    {
        float alpha = color.An;
        float invAlpha = 1f - alpha;

        byte r = (byte)(color.R * alpha + backgroundColor.R * invAlpha);
        byte g = (byte)(color.G * alpha + backgroundColor.G * invAlpha);
        byte b = (byte)(color.B * alpha + backgroundColor.B * invAlpha);
        byte a = (byte)(color.A + backgroundColor.A * invAlpha);

        return new PwColor(r, g, b, a);
    }

    public static PwColor FromHsv(float hue, float saturation, float value)
    {
        int hi = (int)(hue / 60) % 6;
        float f = hue / 60 - (int)(hue / 60);

        value *= 255;
        byte v = (byte)value;
        byte p = (byte)(value * (1 - saturation));
        byte q = (byte)(value * (1 - f * saturation));
        byte t = (byte)(value * (1 - (1 - f) * saturation));

        return hi switch
        {
            0 => new PwColor(v, t, p),
            1 => new PwColor(q, v, p),
            2 => new PwColor(p, v, t),
            3 => new PwColor(p, q, v),
            4 => new PwColor(t, p, v),
            _ => new PwColor(v, p, q),
        };
    }
    
    public byte R;
    public byte G;
    public byte B;
    public byte A;

    public float Rn => (float)R / byte.MaxValue;
    public float Gn => (float)G / byte.MaxValue;
    public float Bn => (float)B / byte.MaxValue;
    public float An => (float)A / byte.MaxValue;

    public PwColor(string hex)
    {
        if (hex.Length == 6) hex = "FF" + hex;
        else if (hex.Length != 8) return;

        string hexA = hex[..2];
        string hexR = hex[2..4];
        string hexG = hex[4..6];
        string hexB = hex[6..];

        byte.TryParse(hexA, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out A);
        byte.TryParse(hexR, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out R);
        byte.TryParse(hexG, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out G);
        byte.TryParse(hexB, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out B);
    }

    public PwColor(Color color) : this(color.R, color.G, color.B, color.A)
    {
        
    }

    public PwColor(SKColor color) : this(color.Red, color.Green, color.Blue, color.Alpha)
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

    public string ToHexString()
        => A == 255 ? $"{R:X2}{G:X2}{B:X2}" : $"{A:X2}{R:X2}{G:X2}{B:X2}";

    public static bool operator ==(PwColor left, PwColor right)
    {
        return left.R == right.R && left.G == right.G && left.B == right.B && left.A == right.A;
    }
    
    public static bool operator !=(PwColor left, PwColor right)
    {
        return left.R != right.R || left.G != right.G || left.B != right.B || left.A != right.A;
    }

    public bool Equals(PwColor other)
    {
        return R == other.R && G == other.G && B == other.B && A == other.A;
    }

    public override bool Equals(object? obj)
    {
        return obj is PwColor other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(R, G, B, A);
    }
}