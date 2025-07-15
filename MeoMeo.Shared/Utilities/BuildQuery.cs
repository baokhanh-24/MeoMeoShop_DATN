namespace MeoMeo.Shared.Utilities;
public class BuildQuery
{
    public static string ToQueryString(object obj)
    {
        var keyValuePairs = new List<string>();

        foreach (var prop in obj.GetType().GetProperties())
        {
            var rawValue = prop.GetValue(obj, null);
            if (rawValue == null)
            {
                continue;
            }

            var key = Uri.EscapeDataString(prop.Name);
            var valueString = ConvertValue(rawValue);
            var value = Uri.EscapeDataString(valueString);
            keyValuePairs.Add($"{key}={value}");
        }
        return string.Join("&", keyValuePairs);
    }
    private static string ConvertValue(object value)
    {
        if (value is DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd THH:mm:ssZ");
        }

        if (value is DateTimeOffset dto)
        {
            return dto.ToString("yyyy-MM-dd  THH:mm:ssZ");
        }

        if (value.GetType().FullName == "System.DateOnly")
        {
            return ((DateOnly)value).ToString("yyyy-MM-dd");
        }
        return value.ToString();
    }
}