using Fischless.Relauncher.Extensions;
using System.Windows;

namespace Fischless.Relauncher.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        this.HideFromAltTab();
    }
}
