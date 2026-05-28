namespace Powerplant.Core.Commands;

public class SetBitmapCommand : Command
{
    private ViewportBitmap _oldBitmap;
    private ViewportBitmap _newBitmap;

    public SetBitmapCommand(ViewportBitmap newBitmap)
    {
        _newBitmap = newBitmap;
    }

    public override void Init()
    {
        _oldBitmap = Bitmap.Copy();
    }

    public override void Run()
    {
        Viewport.SetBitmap(_newBitmap);
    }

    public override void Undo()
    {
        Viewport.SetBitmap(_oldBitmap);
    }
}