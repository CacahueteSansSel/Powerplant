using System;
using Powerplant.Core;

namespace Powerplant.FileFormats;

public abstract class FileFormatBase
{
    public abstract string Name { get; }
    public abstract string[] Extensions { get; }
    
    public virtual bool SupportsExtension(string extension) 
        => Extensions.Contains(extension);

    public abstract void Save(ViewportBitmap bitmap, string filename);
    public abstract ViewportBitmap? Load(string filename);
}