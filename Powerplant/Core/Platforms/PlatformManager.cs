using System;
using Powerplant.Core.Platforms.MacOS;

namespace Powerplant.Core.Platforms;

public static class PlatformManager
{
    public static PlatformBackend Current { get; private set; }

    public static void Init()
    {
        if (OperatingSystem.IsMacOS())
            Current = new MacOSPlatformBackend();
    }
}