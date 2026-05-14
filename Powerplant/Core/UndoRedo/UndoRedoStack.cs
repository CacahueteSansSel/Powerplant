using System.Collections.Generic;
using Powerplant.Controls;

namespace Powerplant.Core.UndoRedo;

public class UndoRedoStack
{
    private Stack<Command> _ranCommands = [];
    private Stack<Command> _undoneCommands = [];
    
    public ViewportControl Viewport { get; private set; }

    public UndoRedoStack(ViewportControl viewport)
    {
        Viewport = viewport;
    }

    public void Push(Command command, bool clearUndone = true)
    {
        command.Viewport = Viewport;
        command.Run();
        
        Viewport.Bitmap.Sync();
        Viewport.InvalidateVisual();
        
        _ranCommands.Push(command);
        
        if (clearUndone) _undoneCommands.Clear();
    }

    public void Undo()
    {
        if (!_ranCommands.TryPop(out Command? command)) return;

        command.Viewport = Viewport;
        command.Undo();
        
        Viewport.Bitmap.Sync();
        Viewport.InvalidateVisual();
        
        _undoneCommands.Push(command);
    }

    public void Redo()
    {
        if (!_undoneCommands.TryPop(out Command? cmd)) return;

        Push(cmd, false);
    }
}