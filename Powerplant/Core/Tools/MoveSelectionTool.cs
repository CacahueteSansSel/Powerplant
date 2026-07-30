using System;
using System.Collections.Generic;
using System.Numerics;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using Powerplant.Core.Commands;
using Tmds.DBus.Protocol;

namespace Powerplant.Core.Tools;

public class MoveSelectionTool : ViewportTool
{
    private bool _isMoving;
    int _initialX, _initialY;
    int _deltaX, _deltaY;
    
    public override string Name => "Move Selection";
    public override Key? Key => Avalonia.Input.Key.M;
    
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

    public override void Render(DrawingContext context)
    {
        if (!_isMoving) return;
        if (Viewport.SelectionGeometry == null) return;

        Matrix matrix = Matrix.Identity * Matrix.CreateTranslation(Viewport.InvertTransformX(_deltaX), 
            Viewport.InvertTransformY(_deltaY));
        
        using (DrawingContext.PushedState ctx = context.PushTransform(matrix))
        {
            context.DrawGeometry(Viewport.SelectionBrush, Viewport.SelectionPen, Viewport.SelectionGeometry);
        }
    }
}