using System.Windows;
using FinanceBuddy.ViewModels;

namespace FinanceBuddy;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}
