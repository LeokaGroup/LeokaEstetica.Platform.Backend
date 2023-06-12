namespace LeokaEstetica.Platform.Models.Dto.Input.Commerce.PayMaster;

/// <summary>
/// Класс входной модели позиций чека.
/// </summary>
public class ReceiptItem
{
    /// <summary>
    /// Название позиции чека.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Кол-во позиций в чеке.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Цена.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Ставка НДС.
    /// </summary>
    public string VatType { get; set; }

    /// <summary>
    /// Предмет расчета.
    /// </summary>
    public string PaymentSubject { get; set; }

    /// <summary>
    /// Способ расчета.
    /// </summary>
    public string PaymentMethod { get; set; }
}