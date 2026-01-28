using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceBuddy.Models;
using FinanceBuddy.Services;

namespace FinanceBuddy.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    private readonly IFinanceRepository _repo;
    private readonly IFinanceSummaryService _summaryService;
    private readonly IAmountParser _amountParser;
    private readonly IEditWindowsService _editWindows;

    public ObservableCollection<Transaction> Transactions => _repo.Transactions;
    public ObservableCollection<string> Categories => _repo.Categories;

    public IReadOnlyList<TransactionType> TransactionTypes { get; } =
        Enum.GetValues(typeof(TransactionType)).Cast<TransactionType>().ToList();

    [ObservableProperty] private TransactionType newType = TransactionType.Expense;
    [ObservableProperty] private string newCategory = "Другое";
    [ObservableProperty] private DateTime? newDate = DateTime.Today;
    [ObservableProperty] private string newDescription = string.Empty;
    [ObservableProperty] private string newAmountText = string.Empty;
    [ObservableProperty] private string validationText = string.Empty;

    [ObservableProperty] private decimal totalIncome;
    [ObservableProperty] private decimal totalExpense;
    public decimal Balance => TotalIncome - TotalExpense;

    [ObservableProperty] private Transaction? selected;
    [ObservableProperty] private string statusText = "готово";

    public MainViewModel(
        IFinanceRepository repo,
        IFinanceSummaryService summaryService,
        IAmountParser amountParser,
        IEditWindowsService editWindows)
    {
        _repo = repo;
        _summaryService = summaryService;
        _amountParser = amountParser;
        _editWindows = editWindows;

        Transactions.CollectionChanged += (_, _) => Recalc();
        Recalc();
    }

    partial void OnSelectedChanged(Transaction? value)
    {
        DeleteSelectedCommand.NotifyCanExecuteChanged();
        EditSelectedCommand.NotifyCanExecuteChanged();
    }

    private void Recalc()
    {
        var summary = _summaryService.Calculate(Transactions);
        TotalIncome = summary.TotalIncome;
        TotalExpense = summary.TotalExpense;
        OnPropertyChanged(nameof(Balance));
    }

    [RelayCommand]
    private void Reload()
    {
        _repo.Reload();
        StatusText = "обновлено";
        Recalc();
    }

    [RelayCommand]
    private void ManageCategories()
    {
        _editWindows.ManageCategories();
        StatusText = "категории: ок";
    }

    [RelayCommand]
    private void AddTransaction()
    {
        ValidationText = string.Empty;

        if (NewDate is null)
        {
            ValidationText = "Укажите дату.";
            return;
        }

        if (!_amountParser.TryParse(NewAmountText, out var amount))
        {
            ValidationText = "Сумма введена неверно. Пример: 199,90";
            return;
        }

        if (amount <= 0)
        {
            ValidationText = "Сумма должна быть больше нуля.";
            return;
        }

        var category = string.IsNullOrWhiteSpace(NewCategory) ? "Другое" : NewCategory.Trim();

        var tx = new Transaction
        {
            Date = NewDate.Value,
            Type = NewType,
            Category = category,
            Description = NewDescription.Trim(),
            Amount = amount
        };

        _repo.AddTransaction(tx);

        NewDescription = string.Empty;
        NewAmountText = string.Empty;

        StatusText = $"добавлено: {tx.Category} {tx.Amount:N2}";
        Recalc();
    }

    private bool CanDeleteSelected() => Selected != null;

    [RelayCommand(CanExecute = nameof(CanDeleteSelected))]
    private void DeleteSelected()
    {
        if (Selected is null)
            return;

        _repo.DeleteTransaction(Selected.Id);
        Selected = null;
        StatusText = "удалено";
        Recalc();
    }

    private bool CanEditSelected() => Selected != null;

    [RelayCommand(CanExecute = nameof(CanEditSelected))]
    private void EditSelected()
    {
        if (Selected is null)
            return;

        // открываем модальное окно редактирования
        var ok = _editWindows.EditTransaction(Selected, Categories.ToList());
        if (!ok)
        {
            StatusText = "редактирование отменено";
            return;
        }

        // сохраняем изменения в БД
        _repo.UpdateTransaction(Selected);
        StatusText = "изменения сохранены";
        Recalc();
    }
}
