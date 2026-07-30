namespace Powerplant.Utilities;

public static class MathUtilities
{
    public static float Clamp(float v, float min, float max)
        => v < min ? min : v > max ? max : v;
    
    public static int Clamp(int v, int min, int max)
        => v < min ? min : v > max ? max : v;

    public static byte ClampByte(int v)
        => (byte)Clamp(v, byte.MinValue, byte.MaxValue);

    public static byte ClampByte(float v)
        => (byte)Clamp(v, byte.MinValue, byte.MaxValue);
}