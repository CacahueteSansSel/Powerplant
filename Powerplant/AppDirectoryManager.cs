using System;
using System.IO;

namespace Powerplant;

public static class AppDirectoryManager
{
    public static string Path { get; private set; }

    public static void Init()
    {
        Path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData,
            Environment.SpecialFolderOption.Create) + "/Powerplant";

        if (!Directory.Exists(Path))
            Directory.CreateDirectory(Path);
    }

    public static string GetDirectory(string name)
    {
        string path = Path + $"/{name}";
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        return path;
    }
}