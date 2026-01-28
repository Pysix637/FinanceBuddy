using System.Windows;
using FinanceBuddy.ViewModels.Dialogs;

namespace FinanceBuddy.Windows;

public partial class TransactionEditWindow : Window
{
    public TransactionEditWindow()
    {
        InitializeComponent();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not TransactionEditViewModel vm)
        {
            DialogResult = false;
            Close();
            return;
        }

        if (!vm.TryValidate(out _))
            return;

        DialogResult = true;
        Close();
    }
}
