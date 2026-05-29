using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform;

namespace Powerplant.Core.Platforms.MacOS;

public class MacOSPlatformBackend : PlatformBackend
{
    public override void SetModifiedFlagOnWindow(Window window, bool modified)
    {
        IPlatformHandle? handle = window.TryGetPlatformHandle();
        if (handle == null)
        {
            Console.WriteLine("Platform handle is null !");
            return;
        }

        nint selector = MacOSInterop.sel_registerName("setDocumentEdited:");

        MacOSInterop.objc_msgSend_bool(handle.Handle, selector, modified);
    }

    public override async Task<bool> ShowConfirmDialog(string title, string message, string yes, string no)
    {
        nint nsAlertClass = MacOSInterop.objc_getClass("NSAlert");
        nint allocSel = MacOSInterop.sel_registerName("alloc");
        nint initSel = MacOSInterop.sel_registerName("init");

        nint alert = MacOSInterop.objc_msgSend(nsAlertClass, allocSel);
        alert = MacOSInterop.objc_msgSend(alert, initSel);

        nint setMessageSel = MacOSInterop.sel_registerName("setMessageText:");
        nint setInfoSel = MacOSInterop.sel_registerName("setInformativeText:");
        nint addButtonSel = MacOSInterop.sel_registerName("addButtonWithTitle:");
        nint runModalSel = MacOSInterop.sel_registerName("runModal");

        MacOSInterop.objc_msgSend_ptr(alert, setMessageSel, MacOSUtilities.CreateNSString(title));
        MacOSInterop.objc_msgSend_ptr(alert, setInfoSel, MacOSUtilities.CreateNSString(message));

        MacOSInterop.objc_msgSend_ptr(alert, addButtonSel, MacOSUtilities.CreateNSString(yes));
        MacOSInterop.objc_msgSend_ptr(alert, addButtonSel, MacOSUtilities.CreateNSString(no));

        return MacOSInterop.objc_msgSend(alert, runModalSel) == new IntPtr(1000);
    }
}