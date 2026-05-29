using System;
using System.Runtime.InteropServices;

namespace Powerplant.Core.Platforms.MacOS;

public static class MacOSInterop
{
    [DllImport("/usr/lib/libobjc.A.dylib")]
    public static extern IntPtr objc_getClass(string name);

    [DllImport("/usr/lib/libobjc.A.dylib")]
    public static extern IntPtr sel_registerName(string name);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend_bool(
        IntPtr receiver,
        IntPtr selector,
        bool value);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    public static extern IntPtr objc_msgSend_ptr(
        IntPtr receiver,
        IntPtr selector,
        IntPtr value);

    [DllImport("/usr/lib/libobjc.A.dylib")]
    public static extern IntPtr objc_msgSend(
        IntPtr receiver,
        IntPtr selector);
}