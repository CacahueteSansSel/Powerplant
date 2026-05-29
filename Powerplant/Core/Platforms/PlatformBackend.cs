using System.Threading.Tasks;
using Avalonia.Controls;

namespace Powerplant.Core.Platforms;

public abstract class PlatformBackend
{
    public abstract void SetModifiedFlagOnWindow(Window window, bool modified);

    public abstract Task<bool> ShowConfirmDialog(string title, string message, string yes, string no);
}