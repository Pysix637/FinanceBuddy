using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceBuddy.Services;

namespace FinanceBuddy.ViewModels.Dialogs;

public sealed partial class CategoryManageViewModel : ObservableObject
{
    private readonly IFinanceRepository _repo;

    public ObservableCollection<string> Categories => _repo.Categories;

    [ObservableProperty] private string newCategoryName = string.Empty;
    [ObservableProperty] private string statusText = string.Empty;
    [ObservableProperty] private string? selected;

    public CategoryManageViewModel(IFinanceRepository repo)
    {
        _repo = repo;
    }

    partial void OnSelectedChanged(string? value)
    {
        DeleteSelectedCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void AddCategory()
    {
        try
        {
            _repo.AddCategory(NewCategoryName);
            StatusText = "категория добавлена";
            NewCategoryName = string.Empty;
        }
        catch (Exception ex)
        {
            StatusText = ex.Message;
        }
    }

    private bool CanDeleteSelected() => !string.IsNullOrWhiteSpace(Selected);

    [RelayCommand(CanExecute = nameof(CanDeleteSelected))]
    private void DeleteSelected()
    {
        if (string.IsNullOrWhiteSpace(Selected))
            return;

        try
        {
            _repo.DeleteCategory(Selected);
            StatusText = "категория удалена";
            Selected = null;
        }
        catch (Exception ex)
        {
            StatusText = ex.Message;
        }
    }
}
