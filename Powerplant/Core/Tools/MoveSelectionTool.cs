using System;
using System.Collections.Generic;
using System.Numerics;
using Powerplant.Core.Commands;
using Tmds.DBus.Protocol;

namespace Powerplant.Core.Tools;

public class MoveSelectionTool : ViewportTool
{
    private bool _isMoving;
    int _initialX, _initialY;
    int _deltaX, _deltaY;
    
    public override void UsePrimary(int cursorX, int cursorY)
    {
        
    }

    public override void UseSecondary(int cursorX, int cursorY)
    {
        
    }

    public override void OnPointerDown(int cursorX, int cursorY)
    {
        if (Viewport.Selection.IsEmpty) return;
        
        if (_isMoving) return;
        _isMoving = true;
        
        _initialX = cursorX;
        _initialY = cursorY;
    }

    public override void OnPointerUp(int cursorX, int cursorY)
    {
        if (!_isMoving) return;
        _isMoving = false;
        
        if (Viewport.Selection.IsEmpty) return;
        
        _deltaX = cursorX - _initialX;
        _deltaY = cursorY - _initialY;
            
        Viewport.RunCommand(new MovePixelsCommand(Viewport.Selection.Pixels, new Vector2(_deltaX, _deltaY)));
    }

    public override void OnPointerMove(int cursorX, int cursorY)
    {
        if (!_isMoving) return;
        
        _deltaX = cursorX - _initialX;
        _deltaY = cursorY - _initialY;
    }

    class MovePixelsCommand : Command
    {
        private ViewportBitmap _oldBitmap;
        private Vector2[] _pixels;
        private Vector2 _delta;
        private PixelSelection _oldSelection;
        
        public MovePixelsCommand(Vector2[] pixels, Vector2 delta)
        {
            _pixels = pixels;
            _delta = delta;
        }

        public override void Init()
        {
            _oldSelection = Viewport.Selection;
            _oldBitmap = Bitmap.Copy();
        }

        public override void Run()
        {
            Dictionary<Vector2, PwColor> colors = [];

            foreach (Vector2 pixel in _pixels)
            {
                colors.Add(pixel, Bitmap.Get((int)pixel.X, (int)pixel.Y));
                Bitmap.Set((int)pixel.X, (int)pixel.Y, PwColor.Transparent);
            }
            
            foreach (KeyValuePair<Vector2, PwColor> kv in colors)
                Bitmap.Set((int)(kv.Key.X + _delta.X), (int)(kv.Key.Y + _delta.Y), kv.Value);

            PixelSelection offsetSelection = Viewport.Selection.Copy();
            offsetSelection.Offset(_delta);
            Viewport.SetSelection(offsetSelection);
        }

        public override void Undo()
        {
            Viewport.SetBitmap(_oldBitmap);
            Viewport.SetSelection(_oldSelection);
        }
    }
}