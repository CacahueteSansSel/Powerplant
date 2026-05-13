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
using Powerplant.Core.Tools;
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
    public PwColor PrimaryColor { get; private set; } = PwColor.Black;
    public PwColor SecondaryColor { get; private set; } = PwColor.White;
    public SolidColorBrush PrimaryColorBrush { get; private set; }
    public SolidColorBrush SecondaryColorBrush { get; private set; }
    public ViewportTool? Tool { get; private set; }
    public event EventHandler<ViewportTool?> OnToolChanged;
    public event EventHandler<PwColor> OnPrimaryColorChanged;
    public event EventHandler<PwColor> OnSecondaryColorChanged;
    
    public ViewportControl()
    {
        RenderOptions.SetBitmapInterpolationMode(
            this,
            BitmapInterpolationMode.None);

        Focusable = true;

        _blackPen = new Pen(0xFF000000);
        _gridPen = new Pen(0xFFAAAAAA);
        
        _bitmap = new ViewportBitmap(16, 16);
        _bitmap.Sync();

        RegisterEvents();
    }

    public void SetTool(ViewportTool? tool)
    {
        Tool = tool;
        Tool?.Viewport = this;
        
        OnToolChanged?.Invoke(this, tool);
    }

    public void SetPrimaryColor(PwColor color)
    {
        PrimaryColor = color;
        PrimaryColorBrush = new SolidColorBrush(color.ToColor());
        
        OnPrimaryColorChanged?.Invoke(this, color);
    }

    public void SetSecondaryColor(PwColor color)
    {
        SecondaryColor = color;
        SecondaryColorBrush = new SolidColorBrush(color.ToColor());
        
        OnSecondaryColorChanged?.Invoke(this, color);
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
        Point pos = e.GetPosition(this);
        PointerPointProperties props = e.GetCurrentPoint(this).Properties;
        
        _dragLastCursorPos = null;
        _dragLastOffset = null;
        
        int imgPosX = TransformCoordX(pos.X);
        int imgPosY = TransformCoordY(pos.Y);
        
        if (props.PointerUpdateKind != PointerUpdateKind.MiddleButtonReleased) 
            Tool?.OnPointerUp(imgPosX, imgPosY);
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        Point pos = e.GetPosition(this);
        PointerPointProperties props = e.GetCurrentPoint(this).Properties;

        if (props.IsMiddleButtonPressed)
        {
            ProcessViewportOffsetDrag(pos);
            return;
        }

        int imgPosX = TransformCoordX(pos.X);
        int imgPosY = TransformCoordY(pos.Y);
        
        Tool?.OnPointerMove(imgPosX, imgPosY);
        
        if (Tool == null || !Tool.SupportsHold)
            return;
        
        if (props.IsLeftButtonPressed) Tool?.UsePrimary(imgPosX, imgPosY);
        else if (props.IsRightButtonPressed) Tool?.UseSecondary(imgPosX, imgPosY);
        else return;
        
        _bitmap.Sync();
        InvalidateVisual();
    }

    public int InvertTransformCoordX(double posX)
    {
        return (int)Math.Floor(posX * Zoom + _offset.X);
    }

    public int InvertTransformCoordY(double posY)
    {
        return (int)Math.Floor(posY * Zoom + _offset.Y);
    }

    public int InvertTransformX(double posX)
    {
        return (int)Math.Floor(posX * Zoom);
    }

    public int InvertTransformY(double posY)
    {
        return (int)Math.Floor(posY * Zoom);
    }

    public int TransformCoordX(double posX)
    {
        return (int)Math.Floor((posX - _offset.X) / Zoom);
    }

    public int TransformCoordY(double posY)
    {
        return (int)Math.Floor((posY - _offset.Y) / Zoom);
    }

    public int TransformX(double posX)
    {
        return (int)Math.Ceiling(posX / Zoom);
    }

    public int TransformY(double posY)
    {
        return (int)Math.Ceiling(posY / Zoom);
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
        
        int imgPosX = TransformCoordX(pos.X);
        int imgPosY = TransformCoordY(pos.Y);
        
        if (!props.IsMiddleButtonPressed) 
            Tool?.OnPointerDown(imgPosX, imgPosY);
        
        if (props.IsLeftButtonPressed) Tool?.UsePrimary(imgPosX, imgPosY);
        else if (props.IsRightButtonPressed) Tool?.UseSecondary(imgPosX, imgPosY);
        else return;
        
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
            for (int x = 0; x <= _bitmap.Width; x++)
            {
                double px = _offset.X + x * Zoom;
                context.DrawLine(_gridPen,
                    new Point(px, _offset.Y),
                    new Point(px, _offset.Y + _bitmap.Height * Zoom));
            }

            for (int y = 0; y <= _bitmap.Height; y++)
            {
                double py = _offset.Y + y * Zoom;
                context.DrawLine(_gridPen,
                    new Point(_offset.X, py),
                    new Point(_offset.X + _bitmap.Width * Zoom, py));
            }
        }
        
        Tool?.Render(context);
    }
}