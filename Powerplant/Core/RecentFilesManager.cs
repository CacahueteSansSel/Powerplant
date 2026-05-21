using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia;

namespace Powerplant.Core;

public static class RecentFilesManager
{
    private static string _recentFilePath;
    private static List<string> _files;

    public static string[] Files => _files.ToArray();
    
    public static void Init()
    {
        _files = [];

        _recentFilePath = AppDirectoryManager.Path + "/recents";

        if (File.Exists(_recentFilePath))
        {
            _files = [..File.ReadAllLines(_recentFilePath)];
            _files.Reverse();

            if (_files.Count > 20)
                _files = _files.Take(20).ToList();
        }
    }

    public static void Save()
    {
        File.WriteAllLines(_recentFilePath, _files);
    }

    public static void Add(string filename)
    {
        if (!File.Exists(filename) || _files.Contains(filename)) return;
        
        _files.Add(filename);
        Save();
    }
}