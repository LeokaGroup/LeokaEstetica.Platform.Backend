using System.Globalization;

namespace LeokaEstetica.Platform.Base.Extensions.PriceExtensions;

/// <summary>
/// Класс расширений для работы с ценами.
/// </summary>
public static class PriceExtensions
{
    #region Публичные методы.

    /// <summary>
    /// Метод разбивает цену на разряды. Цена в исходном виде в строке.
    /// </summary>
    /// <returns>Форматированная по разрядам цена в строке.</returns>
    public static string CreatePriceWithDelimiterFromString(this string price)
    {
        var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
        nfi.NumberGroupSeparator = " ";

        // Цена в виде 1 000.
        var result = Convert.ToDecimal(price).ToString("#,0", nfi);

        return result;
    }
    
    /// <summary>
    /// Метод разбивает цену на разряды. Цена в исходном виде как число Decimal.
    /// </summary>
    /// <returns>Форматированная по разрядам цена в строке.</returns>
    public static string CreatePriceWithDelimiterFromDecimal(this decimal price)
    {
        var nfi = CreateNumberFormatInfo();
        
        // Цена в виде 1 000.
        var result = price.ToString("#,0", nfi);

        return result;
    }
    
    /// <summary>
    /// Метод разбивает цену на разряды. Цена в исходном виде как число Double.
    /// </summary>
    /// <returns>Форматированная по разрядам цена в строке.</returns>
    public static string CreatePriceWithDelimiterFromDecimal(this double price)
    {
        var nfi = CreateNumberFormatInfo();

        // Цена в виде 1 000.
        var result = price.ToString("#,0", nfi);

        return result;
    }

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод создает настроенный объект NumberFormatInfo.
    /// </summary>
    /// <returns>Настроенный NumberFormatInfo.</returns>
    private static NumberFormatInfo CreateNumberFormatInfo()
    {
        var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
        nfi.NumberGroupSeparator = " ";

        return nfi;
    }

    #endregion
}