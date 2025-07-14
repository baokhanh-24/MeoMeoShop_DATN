using System.Globalization;

namespace MeoMeo.Utilities;

public static class FormatCurrency
{
    public static string ToVnCurrency(this decimal value, bool withDecimal = false)
    {
        var culture = new CultureInfo("vi-VN");
        return value.ToString(withDecimal ? "C2" : "C0", culture);
    }
}