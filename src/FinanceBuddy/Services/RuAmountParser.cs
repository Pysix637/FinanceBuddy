using System.Globalization;

namespace FinanceBuddy.Services;

public sealed class RuAmountParser : IAmountParser
{
    public bool TryParse(string text, out decimal amount)
    {
        amount = 0m;

        if (string.IsNullOrWhiteSpace(text))
            return false;

        return decimal.TryParse(text, NumberStyles.Number, CultureInfo.GetCultureInfo("ru-RU"), out amount)
               || decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out amount);
    }
}
