namespace Powerplant.Core.Commands;

public class SelectionCommand : Command
{
    private PixelSelection _oldSelection;
    private PixelSelection _newSelection;

    public SelectionCommand(PixelSelection newSelection)
    {
        _newSelection = newSelection;
    }

    public override void Init()
    {
        _oldSelection = Viewport.Selection;
    }

    public override void Run()
    {
        Viewport.SetSelection(_newSelection);
    }

    public override void Undo()
    {
        Viewport.SetSelection(_oldSelection);
    }
}