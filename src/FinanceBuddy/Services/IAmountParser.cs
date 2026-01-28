namespace FinanceBuddy.Services;

public interface IAmountParser
{
    bool TryParse(string text, out decimal amount);
}
