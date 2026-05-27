namespace Powerplant.Core.Commands;

public class SelectionCommand : Command
{
    private PixelSelection _oldSelection;
    private PixelSelection _newSelection;
    private SelectionMode _mode;

    public SelectionCommand(PixelSelection newSelection, SelectionMode mode)
    {
        _newSelection = newSelection;
        _mode = mode;
    }

    public override void Init()
    {
        _oldSelection = Viewport.Selection;
    }

    public override void Run()
    {
        Viewport.SetSelection(_newSelection, _mode);
    }

    public override void Undo()
    {
        Viewport.SetSelection(_oldSelection);
    }
}

public enum SelectionMode
{
    Set,
    Add,
    Remove
}