using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
using Powerplant.Core.Commands;
using Powerplant.Core.Tools;
using Powerplant.Core.UndoRedo;
using Powerplant.FileFormats;
using Path = Avalonia.Controls.Shapes.Path;
using SelectionMode = Powerplant.Core.Commands.SelectionMode;

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
    private StreamGeometry? _selectionGeometry;
    private IBrush _selectionBrush;
    private Pen _selectionPen;
    private Vector2 _realCursorPos;
    private Vector2? _bitmapCursorPos;
    private Pen _curPixelPen;
    private string _toolDescText;
    private Bitmap _checkerboard;
    private ImageBrush _backgroundBrush;
    private Vector2? _fixedCheckerboardTileSize;
    
    public float Zoom => MathF.Pow(1.1f, _zoom);
    public ViewportBitmap Bitmap => _bitmap;
    public PwColor PrimaryColor { get; private set; } = PwColor.Black;
    public PwColor SecondaryColor { get; private set; } = PwColor.White;
    public SolidColorBrush PrimaryColorBrush { get; private set; }
    public SolidColorBrush SecondaryColorBrush { get; private set; }
    public IBrush? SelectionBrush => _selectionBrush;
    public IPen? SelectionPen => _selectionPen;
    public ViewportTool? Tool { get; private set; }
    public UndoRedoStack UndoRedoStack { get; private set; }
    public PixelSelection Selection { get; private set; } = PixelSelection.Empty;
    public event EventHandler<ViewportTool?> OnToolChanged;
    public event EventHandler<PwColor> OnPrimaryColorChanged;
    public event EventHandler<PwColor> OnSecondaryColorChanged;
    public event EventHandler<ViewportBitmap> OnBitmapChanged;
    public event Action<int, int> OnCursorPositionChanged;
    public event EventHandler<PixelSelection> OnSelectionChanged;
    public event EventHandler<string> OnToolDescriptionTextChanged;
    public event Action OnModification;

    public StreamGeometry? SelectionGeometry => _selectionGeometry;
    public Vector2 RealCursorPosition => _realCursorPos;
    public Vector2? BitmapCursorPosition => _bitmapCursorPos;
    public bool IsShiftPressed { get; private set; }

    public ViewportControl()
    {
        RenderOptions.SetBitmapInterpolationMode(
            this,
            BitmapInterpolationMode.None);

        Focusable = true;

        _blackPen = new Pen(0xFF000000);
        _gridPen = new Pen(0x77000000);
        _selectionBrush = new SolidColorBrush(0x77A7F55D);
        _selectionPen = new Pen(0xFF4D6537, 2, DashStyle.Dash);
        _curPixelPen = new Pen(0xFF000000, 2);

        _checkerboard = new Bitmap(AssetLoader.Open(new Uri("avares://Powerplant/Resources/checkerboard.png")));
        _backgroundBrush = new ImageBrush(_checkerboard)
        {
            TileMode = TileMode.Tile,
            DestinationRect = new RelativeRect(0, 0, 32, 32, RelativeUnit.Absolute),
            Stretch = Stretch.Fill
        };
        
        _bitmap = new ViewportBitmap(16, 16);
        _bitmap.Sync();

        UndoRedoStack = new UndoRedoStack(this);

        RegisterEvents();
    }

    public void SetFixedCheckerboardTileSize(Vector2? tileSize)
    {
        _fixedCheckerboardTileSize = tileSize;
    }

    public void SetToolDescriptionText(string text)
    {
        _toolDescText = text;
        OnToolDescriptionTextChanged?.Invoke(this, _toolDescText);
    }

    public void RunCommand(Command command)
    {
        UndoRedoStack.Push(command);
        OnModification?.Invoke();
    }

    public void SetSelection(PixelSelection selection, SelectionMode mode = SelectionMode.Set)
    {
        switch (mode)
        {
            case SelectionMode.Set:
                Selection = selection;
                
                break;
            case SelectionMode.Add:
                Selection.Add(selection);
                
                break;
            case SelectionMode.Remove:
                throw new NotImplementedException();
                
                break;
        }
        
        OnSelectionChanged?.Invoke(this, selection);
        
        InvalidateVisual();
    }

    public void ClearSelection() 
        => SetSelection(PixelSelection.Empty);

    public ViewportBitmap? GenerateBitmapFromSelection()
    {
        if (Selection.IsEmpty) return null;

        ViewportBitmap bitmap = new ViewportBitmap((int)Selection.Bounds.Width, (int)Selection.Bounds.Height);
        foreach (Vector2 pixel in Selection.Pixels)
        {
            bitmap.Set((int)(pixel.X - Selection.Bounds.X), (int)(pixel.Y - Selection.Bounds.Y), 
                Bitmap.Get((int)pixel.X, (int)pixel.Y));
        }
        bitmap.Sync();

        return bitmap;
    }

    private void BuildSelectionGeometry()
    {
        _selectionGeometry = new StreamGeometry();
        
        HashSet<Vector2> pixels = Selection.Pixels.ToHashSet();
        List<(Point A, Point B)> edges = [];

        foreach (Vector2 p in pixels)
        {
            float x = p.X;
            float y = p.Y;

            // Top
            if (!pixels.Contains(new Vector2(x, y - 1)))
            {
                edges.Add((
                    new Point(x, y),
                    new Point(x + 1, y)
                ));
            }

            // Right
            if (!pixels.Contains(new Vector2(x + 1, y)))
            {
                edges.Add((
                    new Point(x + 1, y),
                    new Point(x + 1, y + 1)
                ));
            }

            // Bottom
            if (!pixels.Contains(new Vector2(x, y + 1)))
            {
                edges.Add((
                    new Point(x + 1, y + 1),
                    new Point(x, y + 1)
                ));
            }

            // Left
            if (!pixels.Contains(new Vector2(x - 1, y)))
            {
                edges.Add((
                    new Point(x, y + 1),
                    new Point(x, y)
                ));
            }
        }
        
        using StreamGeometryContext ctx = _selectionGeometry.Open();
        List<(Point A, Point B)> remaining = new List<(Point A, Point B)>(edges);

        while (remaining.Count > 0)
        {
            (Point A, Point B) first = remaining[0];
            remaining.RemoveAt(0);

            List<Point> polygon =
            [
                first.A,
                first.B
            ];

            Point current = first.B;

            while (true)
            {
                int index = remaining.FindIndex(e => e.A == current);
                if (index == -1) break;

                (Point A, Point B) edge = remaining[index];
                remaining.RemoveAt(index);

                polygon.Add(edge.B);
                current = edge.B;

                if (current == polygon[0]) break;
            }

            ctx.BeginFigure(new Point(_offset.X + polygon[0].X * Zoom, _offset.Y + polygon[0].Y * Zoom));
            for (int i = 1; i < polygon.Count; i++)
            {
                ctx.LineTo(new Point(_offset.X + polygon[i].X * Zoom, 
                    _offset.Y + polygon[i].Y * Zoom));
            }
            ctx.EndFigure(true);
        }
    }

    public void SetTool(ViewportTool? tool)
    {
        Tool = tool;
        Tool?.Viewport = this;
        
        OnToolChanged?.Invoke(this, tool);
        
        SetToolDescriptionText(tool == null ? "-" : tool.Name.ToLower());
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
        
        OnBitmapChanged?.Invoke(this, _bitmap);
        
        Center();
    }

    public void SetBitmap(ViewportBitmap bitmap)
    {
        _bitmap = bitmap;
        _bitmap.Sync();
        
        OnBitmapChanged?.Invoke(this, _bitmap);
        
        Center();
        
        OnModification?.Invoke();
    }

    public void LoadTexture(string filename)
    {
        FileFormatBase? ff = FileFormatManager.GetByExtension(System.IO.Path.GetExtension(filename).TrimStart('.'));
        if (ff == null) return;

        _bitmap = ff.Load(filename)!;
        _bitmap.Sync();
        
        OnBitmapChanged?.Invoke(this, _bitmap);
        
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
        KeyDown += OnKeyDown;
        KeyUp += OnKeyUp;
        LostFocus += OnLostFocus;
    }

    private void OnLostFocus(object? sender, FocusChangedEventArgs e)
    {
        IsShiftPressed = false;
        //Console.WriteLine($"Shift: {IsShiftPressed}");
    }

    private void OnKeyUp(object? sender, KeyEventArgs e)
    {
        IsShiftPressed = false;
        //Console.WriteLine($"Shift: {IsShiftPressed}");
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        IsShiftPressed = e.KeyModifiers.HasFlag(KeyModifiers.Shift);
        //Console.WriteLine($"Shift: {IsShiftPressed}");
        
        switch (e.Key)
        {
            case Key.Escape:
                ClearSelection();
                break;
        }

        if (!Selection.IsEmpty)
        {
            switch (e.Key)
            {
                case Key.Delete:
                case Key.Back:
                    ClearSelectionOnImage(true);
                    break;
            }
        }
    }

    private void ClearSelectionOnImage(bool clearSelection = false)
    {
        RunCommand(new PixelsCommand(Selection.Pixels, PwColor.Transparent));
        if (clearSelection) ClearSelection();
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

        _realCursorPos = new Vector2((float)pos.X, (float)pos.Y);

        if (props.IsMiddleButtonPressed)
        {
            ProcessViewportOffsetDrag(pos);
            return;
        }

        int imgPosX = TransformCoordX(pos.X);
        int imgPosY = TransformCoordY(pos.Y);
        
        Tool?.OnPointerMove(imgPosX, imgPosY);
        OnCursorPositionChanged?.Invoke(imgPosX, imgPosY);
        _bitmapCursorPos = new Vector2(imgPosX, imgPosY);
        
        InvalidateVisual();
        
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

        if (_fixedCheckerboardTileSize != null)
        {
            _backgroundBrush.DestinationRect = new RelativeRect(-bounds.X, -bounds.Y, _fixedCheckerboardTileSize.Value.X * 2f * Zoom, _fixedCheckerboardTileSize.Value.Y * 2f * Zoom, RelativeUnit.Absolute);
        }
        else
        {
            _backgroundBrush.DestinationRect = new RelativeRect(-bounds.X, -bounds.Y, 32, 32, RelativeUnit.Absolute);
        }
        
        context.DrawRectangle(_backgroundBrush, null, bounds);
        
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

            bool cursorPosInBounds = _bitmapCursorPos != null
                                     && _bitmapCursorPos.Value.X >= 0 && _bitmapCursorPos.Value.Y >= 0
                                     && _bitmapCursorPos.Value.X < _bitmap.Width
                                     && _bitmapCursorPos.Value.Y < _bitmap.Height;

            if (_bitmapCursorPos != null && cursorPosInBounds)
            {
                Rect selectionRect = new Rect(InvertTransformCoordX(_bitmapCursorPos.Value.X),
                    InvertTransformCoordY(_bitmapCursorPos.Value.Y), Zoom, Zoom);
                
                context.DrawRectangle(_curPixelPen, selectionRect);
            }
        }
        
        if (!Selection.IsEmpty)
        {
            BuildSelectionGeometry();
            context.DrawGeometry(_selectionBrush, _selectionPen, _selectionGeometry);
        }
        
        Tool?.Render(context);
    }

    public void DecrementZoom()
    {
        _zoom -= 2f;
        _offset = new Vector2((float)Bounds.Width / 2 - _bitmap.Width * Zoom / 2, 
            (float)Bounds.Height / 2 - _bitmap.Height * Zoom / 2);
        
        InvalidateVisual();
    }

    public void IncrementZoom()
    {
        _zoom += 2f;
        _offset = new Vector2((float)Bounds.Width / 2 - _bitmap.Width * Zoom / 2, 
            (float)Bounds.Height / 2 - _bitmap.Height * Zoom / 2);
        
        InvalidateVisual();
    }
}