namespace Powerplant.Core.Commands;

public class FlipCommand : Command
{
    private ViewportBitmap _oldBitmap;
    private bool _horizontal;

    public FlipCommand(bool isHorizontal)
    {
        _horizontal = isHorizontal;
    }

    public override void Init()
    {
        _oldBitmap = Bitmap.Copy();
    }

    public override void Run()
    {
        if (_horizontal) Bitmap.FlipHorizontal();
        else Bitmap.FlipVertical();
        
        Bitmap.Sync();
    }

    public override void Undo()
    {
        Viewport.SetBitmap(_oldBitmap);
    }
}