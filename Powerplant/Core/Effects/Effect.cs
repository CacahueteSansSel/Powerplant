namespace Powerplant.Core.Effects;

public abstract class Effect
{
    public abstract bool Apply(ViewportBitmap referenceBitmap, ViewportBitmap targetBitmap);
}