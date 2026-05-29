using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Powerplant.Core;
using Powerplant.Core.Effects;
using Powerplant.Core.Platforms;
using Powerplant.FileFormats;
using Powerplant.Windows;

namespace Powerplant;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        PlatformManager.Init();
        FileFormatManager.Init();
        AppDirectoryManager.Init();
        RecentFilesManager.Init();
        EffectsManager.Init();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private async void NativeMenuItem_OnClick(object? sender, EventArgs e)
    {
        new AboutWindow().Show();
    }
}