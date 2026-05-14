using Avalonia.Media.Imaging;

namespace Powerplant.Core.Commands;

public class ResizeImageCommand : Command
{
    private int _newWidth, _newHeight;
    private BitmapInterpolationMode _interpolationMode;
    private ViewportBitmap _oldBitmap;

    public ResizeImageCommand(int newWidth, int newHeight, BitmapInterpolationMode interpolationMode)
    {
        _newWidth = newWidth;
        _newHeight = newHeight;
        _interpolationMode = interpolationMode;
    }

    public override void Init()
    {
        _oldBitmap = Bitmap.Copy();
    }

    public override void Run()
    {
        Viewport.SetBitmap(Bitmap.Scale(_newWidth, _newHeight, _interpolationMode));
    }

    public override void Undo()
    {
        Viewport.SetBitmap(_oldBitmap);
    }
}