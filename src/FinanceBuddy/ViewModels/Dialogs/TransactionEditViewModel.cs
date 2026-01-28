using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceBuddy.Models;
using FinanceBuddy.Services;

namespace FinanceBuddy.ViewModels.Dialogs;

public sealed partial class TransactionEditViewModel : ObservableObject
{
    private readonly IAmountParser _amountParser;

    public TransactionEditViewModel(IAmountParser amountParser)
    {
        _amountParser = amountParser;
        TransactionTypes = Enum.GetValues(typeof(TransactionType)).Cast<TransactionType>().ToList();
        Categories = new List<string>();
    }

    public IReadOnlyList<TransactionType> TransactionTypes { get; }
    public IReadOnlyList<string> Categories { get; private set; }

    [ObservableProperty] private TransactionType type;
    [ObservableProperty] private string category = "Другое";
    [ObservableProperty] private DateTime? date = DateTime.Today;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private string amountText = string.Empty;

    [ObservableProperty] private string validationText = string.Empty;

    public void LoadFrom(Transaction tx, IReadOnlyList<string> categories)
    {
        Categories = categories.ToList();
        OnPropertyChanged(nameof(Categories));

        Type = tx.Type;
        Category = tx.Category;
        Date = tx.Date;
        Description = tx.Description;
        AmountText = tx.Amount.ToString("N2");
        ValidationText = string.Empty;
    }

    public void ApplyTo(Transaction tx)
    {
        tx.Type = Type;
        tx.Category = string.IsNullOrWhiteSpace(Category) ? "Другое" : Category.Trim();
        tx.Date = Date ?? DateTime.Today;
        tx.Description = (Description ?? string.Empty).Trim();
        tx.Amount = ParseAmountOrThrow();
    }

    public bool TryValidate(out decimal amount)
    {
        amount = 0m;
        ValidationText = string.Empty;

        if (Date is null)
        {
            ValidationText = "Укажите дату.";
            return false;
        }

        if (!_amountParser.TryParse(AmountText, out amount))
        {
            ValidationText = "Сумма введена неверно. Пример: 199,90";
            return false;
        }

        if (amount <= 0)
        {
            ValidationText = "Сумма должна быть больше нуля.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(Category))
        {
            ValidationText = "Укажите категорию.";
            return false;
        }

        return true;
    }

    private decimal ParseAmountOrThrow()
    {
        if (!TryValidate(out var amount))
            throw new InvalidOperationException(ValidationText);

        return amount;
    }
}
