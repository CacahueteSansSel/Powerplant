using System;
using System.Runtime.InteropServices;

namespace Powerplant.Core.Platforms.MacOS;

public static class MacOSUtilities
{
    public static IntPtr CreateNSString(string input)
    {
        IntPtr cls = MacOSInterop.objc_getClass("NSString");
        IntPtr alloc = MacOSInterop.sel_registerName("alloc");
        IntPtr init = MacOSInterop.sel_registerName("initWithUTF8String:");
        IntPtr ptr = MacOSInterop.objc_msgSend(cls, alloc);

        return MacOSInterop.objc_msgSend_ptr(ptr, init, Marshal.StringToHGlobalAuto(input));
    }
}