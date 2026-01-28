using FinanceBuddy.Models;
using FinanceBuddy.ViewModels.Dialogs;
using FinanceBuddy.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceBuddy.Services;

public sealed class EditWindowsService : IEditWindowsService
{
    private readonly IServiceProvider _sp;

    public EditWindowsService(IServiceProvider sp)
    {
        _sp = sp;
    }

    public bool EditTransaction(Transaction tx, IReadOnlyList<string> categories)
    {
        var vm = _sp.GetRequiredService<TransactionEditViewModel>();
        vm.LoadFrom(tx, categories);

        var window = _sp.GetRequiredService<TransactionEditWindow>();
        window.DataContext = vm;
        window.Owner = System.Windows.Application.Current.MainWindow;

        var result = window.ShowDialog();
        if (result != true)
            return false;

        vm.ApplyTo(tx);
        return true;
    }

    public void ManageCategories()
    {
        var vm = _sp.GetRequiredService<CategoryManageViewModel>();
        var window = _sp.GetRequiredService<CategoryManageWindow>();
        window.DataContext = vm;
        window.Owner = System.Windows.Application.Current.MainWindow;
        window.ShowDialog();
    }
}
