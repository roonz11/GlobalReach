namespace GlobalReach.Helpers
{
    public static class CurrencyHelper
    {
        public static string DisplayAs(double amount, string currency)
        {
            return string.Format($"{amount.ToString("N2")} {currency.ToUpper()}");
        }
    }
}
