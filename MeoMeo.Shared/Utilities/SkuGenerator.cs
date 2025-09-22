namespace MeoMeo.Shared.Utilities;

public static class SkuGenerator
{
    public static IEnumerable<string> GenerateRange(string? latestSku, int count, string prefix = "PRO")
    {
        long startNumber = 0;

        if (!string.IsNullOrWhiteSpace(latestSku) && latestSku.StartsWith(prefix))
        {
            string numberPart = latestSku.Substring(prefix.Length);
            if (long.TryParse(numberPart, out long parsed))
            {
                startNumber = parsed;
            }
        }

        for (int i = 1; i <= count; i++)
        {
            yield return prefix + (startNumber + i);
        }
    }
}