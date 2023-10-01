namespace LeokaEstetica.Platform.Core.Utils;

public static class TextHelper
{
    public static string[] SplitBySeparators(this string text, params string[] separators) 
    {
        if (string.IsNullOrEmpty(text))
        {
            return new string[0];
        }

        return text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
    }
}