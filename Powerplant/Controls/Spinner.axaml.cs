using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Powerplant.Controls;

public partial class Spinner : UserControl
{
    public event Action<int>? Spinned;
    
    public Spinner()
    {
        InitializeComponent();
    }

    private void SpinUpButtonClicked(object? sender, RoutedEventArgs e)
    {
        Spinned?.Invoke(1);
    }

    private void SpinDownButtonClicked(object? sender, RoutedEventArgs e)
    {
        Spinned?.Invoke(-1);
    }
}