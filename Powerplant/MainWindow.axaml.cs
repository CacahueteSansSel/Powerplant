using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Powerplant.Core;
using Powerplant.FileFormats;
using Powerplant.Windows;

namespace Powerplant;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void MenuNewTextureOptionClicked(object? sender, EventArgs e)
    {
        Vector2 size = await new NewTextureWindow().ShowDialog<Vector2>(this);
        if (size.X == 0 || size.Y == 0) return;
        
        Viewport.CreateTexture((int)size.X, (int)size.Y);
    }

    private async void MenuOpenTextureOptionClicked(object? sender, EventArgs e)
    {
        List<FilePickerFileType> fileTypes = FileFormatManager.BuildFilePickerFileList();

        IReadOnlyList<IStorageFile> fileList = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            FileTypeFilter = fileTypes
        });
        
        if (fileList.Count != 1) return;
        IStorageFile file = fileList.First();
        
        Viewport.LoadTexture(file.Path.AbsolutePath);
    }

    private async void MenuSaveTextureOptionClicked(object? sender, EventArgs e)
    {
        List<FilePickerFileType> fileTypes = FileFormatManager.BuildFilePickerFileList();

        IStorageFile? file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
        {
            FileTypeChoices = fileTypes
        });
        if (file == null) return;
        
        FileFormatBase? ff = FileFormatManager.GetByExtension(System.IO.Path.GetExtension(file.Path.AbsolutePath).TrimStart('.'));
        if (ff == null) return;
        
        ff.Save(Viewport.Bitmap, file.Path.AbsolutePath);
    }
}