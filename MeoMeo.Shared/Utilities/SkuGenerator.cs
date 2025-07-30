namespace MeoMeo.Shared.Utilities;

public static class SkuGenerator
{
    public static string GenerateNextSku(string? latestSku, string prefix = "PRO", int totalLength = 10)
    {
        int numberLength = totalLength - prefix.Length;
        int nextNumber = 1;

        if (!string.IsNullOrWhiteSpace(latestSku) && latestSku.StartsWith(prefix))
        {
            string numberPart = latestSku.Substring(prefix.Length);
            if (int.TryParse(numberPart, out int parsed))
            {
                nextNumber = parsed + 1;
            }
        }

        string paddedNumber = nextNumber.ToString().PadLeft(numberLength, '0');
        return prefix + paddedNumber;
    }
}
