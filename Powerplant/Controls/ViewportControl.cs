using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Powerplant.Core;
using Powerplant.FileFormats;
using Path = Avalonia.Controls.Shapes.Path;

namespace Powerplant.Controls;

public class ViewportControl : Control
{
    private ViewportBitmap _bitmap;
    private float _zoom = 2f;
    private Pen _blackPen;
    private Pen _gridPen;
    private Vector2 _offset;
    private Point? _dragLastCursorPos;
    private Vector2? _dragLastOffset;
    
    public float Zoom => MathF.Pow(1.1f, _zoom);
    public ViewportBitmap Bitmap => _bitmap;
    
    public ViewportControl()
    {
        RenderOptions.SetBitmapInterpolationMode(
            this,
            BitmapInterpolationMode.None);

        Focusable = true;

        _blackPen = new Pen(0xFF000000);
        _gridPen = new Pen(0xFFAAAAAA);
        
        _bitmap = new ViewportBitmap(100, 100);
        
        _bitmap.Set(0, 0, PwColor.Black);
        _bitmap.Set(2, 2, PwColor.White);
        _bitmap.Set(4, 4, PwColor.Red);
        _bitmap.Set(6, 6, PwColor.Green);
        _bitmap.Set(8, 8, PwColor.Blue);
        
        _bitmap.Sync();

        RegisterEvents();
    }

    public void CreateTexture(int width, int height)
    {
        _bitmap = new ViewportBitmap(width, height);
        _bitmap.Sync();
        
        Center();
    }

    public void LoadTexture(string filename)
    {
        FileFormatBase? ff = FileFormatManager.GetByExtension(System.IO.Path.GetExtension(filename).TrimStart('.'));
        if (ff == null) return;

        _bitmap = ff.Load(filename)!;
        _bitmap.Sync();
        
        Center();
    }

    public void Center()
    {
        float zoomX = (float)Bounds.Width / _bitmap.Width;
        float zoomY = (float)Bounds.Height / _bitmap.Height;

        _zoom = MathF.Log(MathF.Min(zoomX, zoomY), 1.1f);
        
        _offset = new Vector2((float)Bounds.Width / 2 - _bitmap.Width * Zoom / 2, 
            (float)Bounds.Height / 2 - _bitmap.Height * Zoom / 2);
        
        InvalidateVisual();
    }

    private void RegisterEvents()
    {
        PointerWheelChanged += OnPointerWheelChanged;
        PointerPressed += OnPointerPressed;
        PointerMoved += OnPointerMoved;
        PointerReleased += OnPointerReleased;
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _dragLastCursorPos = null;
        _dragLastOffset = null;
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        Point pos = e.GetPosition(this);
        PointerPointProperties props = e.GetCurrentPoint(this).Properties;
        PwColor color = new PwColor();

        if (props.IsMiddleButtonPressed) ProcessViewportOffsetDrag(pos);
        
        if (props.IsLeftButtonPressed) color = PwColor.Blue;
        else if (props.IsRightButtonPressed) color = PwColor.Black;
        else return;

        int imgPosX = (int)((pos.X - _offset.X) / Zoom);
        int imgPosY = (int)((pos.Y - _offset.Y) / Zoom);
        _bitmap.Set(imgPosX, imgPosY, color);
        _bitmap.Sync();
        InvalidateVisual();
    }

    private void ProcessViewportOffsetDrag(Point pos)
    {
        if (_dragLastCursorPos == null)
        {
            _dragLastCursorPos = pos;
            _dragLastOffset = _offset;
        }
        
        if (!_dragLastCursorPos.HasValue || !_dragLastOffset.HasValue) return;

        Point delta = pos - _dragLastCursorPos.Value;
        _offset.X = (int)(_dragLastOffset.Value.X + delta.X);
        _offset.Y = (int)(_dragLastOffset.Value.Y + delta.Y);
        
        InvalidateVisual();
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        Point pos = e.GetPosition(this);
        PointerPointProperties props = e.GetCurrentPoint(this).Properties;
        PwColor color = new PwColor();
        
        if (props.IsLeftButtonPressed) color = PwColor.Blue;
        else if (props.IsRightButtonPressed) color = PwColor.Black;
        else return;

        int imgPosX = (int)((pos.X - _offset.X) / Zoom);
        int imgPosY = (int)((pos.Y - _offset.Y) / Zoom);
        _bitmap.Set(imgPosX, imgPosY, color);
        _bitmap.Sync();
        InvalidateVisual();
    }

    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        Point cursorPos = e.GetPosition(this);
        
        float oldZoom = Zoom;

        float oldOffsetX = (float)((cursorPos.X - _offset.X) / oldZoom);
        float oldOffsetY = (float)((cursorPos.Y - _offset.Y) / oldZoom);
        
        if (e.Delta.Y < 0 && _zoom > 2f) _zoom -= 2f;
        if (e.Delta.Y > 0) _zoom += 2f;

        _offset.X = (float)(cursorPos.X - oldOffsetX * Zoom);
        _offset.Y = (float)(cursorPos.Y - oldOffsetY * Zoom);
        
        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        bool renderGrid = _zoom > 30;
        
        context.DrawRectangle(new SolidColorBrush(Colors.White), null, new Rect(0, 0, Bounds.Width, Bounds.Height));

        Rect bounds = new Rect(_offset.X, _offset.Y, _bitmap.Width * Zoom, _bitmap.Height * Zoom);
        context.DrawImage(_bitmap.Image, bounds);
        context.DrawRectangle(null, _blackPen, bounds);

        if (renderGrid)
        {
            for (int y = 0; y < _bitmap.Height; y++)
            {
                for (int x = 0; x < _bitmap.Width; x++)
                {
                    Rect pixelRect = new(_offset.X + x * Zoom, _offset.Y + y * Zoom, Zoom, Zoom);
                    context.DrawRectangle(null, _gridPen, pixelRect);
                }
            }
        }
    }
}