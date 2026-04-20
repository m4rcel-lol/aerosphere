using AeroSphere.App.ViewModels;
using Microsoft.UI.Xaml;
using Windows.Graphics;

namespace AeroSphere.App;

public sealed partial class MainWindow : Window
{
    public DashboardViewModel ViewModel { get; } = new();

    public MainWindow()
    {
        InitializeComponent();
        Title = "AeroSphere";
        AppWindow.Resize(new SizeInt32(1440, 920));
    }
}
