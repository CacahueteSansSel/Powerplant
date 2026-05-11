using System.Collections.Generic;
using System.Linq;
using Avalonia.Platform.Storage;
using Powerplant.FileFormats.ImageSharp;

namespace Powerplant.FileFormats;

public static class FileFormatManager
{
    private static List<FileFormatBase> _fileFormats = [];

    public static FileFormatBase[] FileFormats => _fileFormats.ToArray();

    public static void Init()
    {
        _fileFormats.Add(new PngFileFormat());
    }

    public static FileFormatBase? GetByExtension(string extension)
        => _fileFormats.FirstOrDefault(ff => ff.SupportsExtension(extension));

    public static List<FilePickerFileType> BuildFilePickerFileList()
    {
        List<FilePickerFileType> list = [];

        foreach (FileFormatBase fileFormat in _fileFormats)
        {
            list.Add(new FilePickerFileType(fileFormat.Name)
            {
                Patterns = fileFormat.Extensions.Select(ext => $"*.{ext}").ToArray()
            });
        }
        
        return list;
    }
}