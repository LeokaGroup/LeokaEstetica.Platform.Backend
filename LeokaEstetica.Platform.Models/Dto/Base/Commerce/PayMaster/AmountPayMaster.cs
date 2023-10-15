namespace LeokaEstetica.Platform.Models.Dto.Base.Commerce.PayMaster;

/// <summary>
/// Класс описывает цену.
/// </summary>
public class AmountPayMaster
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="currency"></param>
    public AmountPayMaster(decimal value, string currency)
    {
        Value = value;
        Currency = currency;
    }

    /// <summary>
    /// Сумма.
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// Валюта.
    /// </summary>
    public string Currency { get; set; }
}