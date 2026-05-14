using System;
using Powerplant.Windows;

namespace Powerplant.Core.Commands;

public class ResizeViewportCommand : Command
{
    private ViewportBitmap _oldBitmap;
    private int _newWidth, _newHeight;
    private ResizeAnchor _anchor;

    public ResizeViewportCommand(int newWidth, int newHeight, ResizeAnchor anchor)
    {
        _newWidth = newWidth;
        _newHeight = newHeight;
        _anchor = anchor;
    }

    public override void Init()
    {
        _oldBitmap = Viewport.Bitmap.Copy();
    }

    public override void Run()
    {
        Viewport.CreateTexture(_newWidth, _newHeight);

        switch (_anchor)
        {
            case ResizeAnchor.TopLeft:
                Viewport.Bitmap.ApplyBitmap(_oldBitmap, 0, 0);
                
                break;
            case ResizeAnchor.TopMiddle:
                Viewport.Bitmap.ApplyBitmap(_oldBitmap, _newWidth / 2 - _oldBitmap.Width / 2, 0);
                
                break;
            case ResizeAnchor.TopRight:
                Viewport.Bitmap.ApplyBitmap(_oldBitmap, _newWidth - _oldBitmap.Width, 0);
                
                break;
            case ResizeAnchor.MiddleLeft:
                Viewport.Bitmap.ApplyBitmap(_oldBitmap, 0, _newHeight / 2 - _oldBitmap.Height / 2);
                
                break;
            case ResizeAnchor.Center:
                Viewport.Bitmap.ApplyBitmap(_oldBitmap, _newWidth / 2 - _oldBitmap.Width / 2, _newHeight / 2 - _oldBitmap.Height / 2);
                
                break;
            case ResizeAnchor.MiddleRight:
                Viewport.Bitmap.ApplyBitmap(_oldBitmap, _newWidth - _oldBitmap.Width, _newHeight / 2 - _oldBitmap.Height / 2);
                
                break;
            case ResizeAnchor.BottomLeft:
                Viewport.Bitmap.ApplyBitmap(_oldBitmap, 0, _newHeight - _oldBitmap.Height);
                
                break;
            case ResizeAnchor.BottomMiddle:
                Viewport.Bitmap.ApplyBitmap(_oldBitmap, _newWidth / 2 - _oldBitmap.Width / 2, _newHeight - _oldBitmap.Height);
                
                break;
            case ResizeAnchor.BottomRight:
                Viewport.Bitmap.ApplyBitmap(_oldBitmap, _newWidth - _oldBitmap.Width, _newHeight - _oldBitmap.Height);
                
                break;
        }
    }

    public override void Undo()
    {
        Viewport.SetBitmap(_oldBitmap);
    }
}