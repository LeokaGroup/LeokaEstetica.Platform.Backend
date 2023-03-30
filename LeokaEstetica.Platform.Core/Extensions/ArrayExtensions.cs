using System.Globalization;
using LeokaEstetica.Platform.Core.Utils;

namespace LeokaEstetica.Platform.Core.Extensions;

public static class ArrayExtensions
{
    public static TOutput GetStringAsArray<TOutput>(this string source)
    {
        var strArray = source.Trim(new[] { '[', ']' }).Replace(" ", string.Empty).SplitBySeparators(",", ";");
        var resultType = typeof(TOutput);
        var elementType = resultType.GetElementType();

        //// Do add your type.
        if (elementType == typeof(int))
        {
            return strArray.ChangeType<string, TOutput, int>(x => Convert.ToInt32(x));
        }
        else if (elementType == typeof(string))
        {
            return strArray.ChangeType<string, TOutput, string>(x => x);
        }
        else if (elementType == typeof(long))
        {
            return strArray.ChangeType<string, TOutput, long>(x => long.Parse(x));
        }
        else
        {
            throw new ArgumentException(
                $"The type {typeof(TOutput)} is not supported for conversion. Do add your conversion logic.");
        }
    }
    
    private static TOutput ChangeType<TInput, TOutput, TOutputItem>(this TInput[] source, Func<TInput, TOutputItem> converter)
    {
        if (converter != null)
        {
            var resultArray = Array.ConvertAll(source, x => converter(x));
            return (TOutput)Convert.ChangeType(resultArray, resultArray.GetType(), CultureInfo.InvariantCulture);
        }
        else
        {
            return (TOutput)Convert.ChangeType(source, source.GetType(), CultureInfo.InvariantCulture);
        }
    }
}