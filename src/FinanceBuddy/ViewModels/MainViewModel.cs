using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using FinanceBuddy.Helpers;
using FinanceBuddy.Models;
using FinanceBuddy.Services;

namespace FinanceBuddy.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    private readonly InMemoryFinanceRepository _repo = new();

    public ObservableCollection<Transaction> Transactions => _repo.Transactions;
    public ObservableCollection<string> Categories => _repo.Categories;

    public IReadOnlyList<TransactionType> TransactionTypes { get; } =
        Enum.GetValues(typeof(TransactionType)).Cast<TransactionType>().ToList();

    private TransactionType _newType = TransactionType.Expense;
    public TransactionType NewType { get => _newType; set { if (Set(ref _newType, value)) Recalc(); } }

    private string _newCategory = "Другое";
    public string NewCategory { get => _newCategory; set => Set(ref _newCategory, value); }

    private DateTime? _newDate = DateTime.Today;
    public DateTime? NewDate { get => _newDate; set => Set(ref _newDate, value); }

    private string _newDescription = "";
    public string NewDescription { get => _newDescription; set => Set(ref _newDescription, value); }

    private string _newAmountText = "";
    public string NewAmountText { get => _newAmountText; set => Set(ref _newAmountText, value); }

    private string _validationText = "";
    public string ValidationText { get => _validationText; set => Set(ref _validationText, value); }

    private decimal _totalIncome;
    public decimal TotalIncome { get => _totalIncome; private set => Set(ref _totalIncome, value); }

    private decimal _totalExpense;
    public decimal TotalExpense { get => _totalExpense; private set => Set(ref _totalExpense, value); }

    public decimal Balance => TotalIncome - TotalExpense;

    private Transaction? _selected;
    public Transaction? Selected
    {
        get => _selected;
        set { if (Set(ref _selected, value)) ((RelayCommand)DeleteSelectedCommand).RaiseCanExecuteChanged(); }
    }

    private string _statusText = "готово";
    public string StatusText { get => _statusText; set => Set(ref _statusText, value); }

    public ICommand AddTransactionCommand { get; }
    public ICommand DeleteSelectedCommand { get; }

    public MainViewModel()
    {
        AddTransactionCommand = new RelayCommand(AddTransaction);
        DeleteSelectedCommand = new RelayCommand(DeleteSelected, () => Selected != null);

        Transactions.CollectionChanged += (_, _) => Recalc();
        Recalc();
    }

    private void AddTransaction()
    {
        ValidationText = "";

        if (NewDate is null)
        {
            ValidationText = "Укажите дату.";
            return;
        }

        if (!decimal.TryParse(NewAmountText, NumberStyles.Number, CultureInfo.GetCultureInfo("ru-RU"), out var amount) &&
            !decimal.TryParse(NewAmountText, NumberStyles.Number, CultureInfo.InvariantCulture, out amount))
        {
            ValidationText = "Сумма введена неверно. Пример: 199,90";
            return;
        }

        if (amount <= 0)
        {
            ValidationText = "Сумма должна быть больше нуля.";
            return;
        }

        var tx = new Transaction
        {
            Date = NewDate.Value,
            Type = NewType,
            Category = string.IsNullOrWhiteSpace(NewCategory) ? "Другое" : NewCategory.Trim(),
            Description = NewDescription?.Trim() ?? "",
            Amount = amount
        };

        _repo.AddTransaction(tx);

        NewDescription = "";
        NewAmountText = "";
        StatusText = $"добавлено: {tx.Category} {tx.Amount:N2}";
        Recalc();
    }

    private void DeleteSelected()
    {
        if (Selected == null) return;
        _repo.DeleteTransaction(Selected.Id);
        StatusText = "удалено";
        Recalc();
    }

    private void Recalc()
    {
        TotalIncome = Transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
        TotalExpense = Transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
        Raise(nameof(Balance));
    }
}
