namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление типов заказа.
/// </summary>
public enum OrderTypeEnum
{
    /// <summary>
    /// Неизвестный тип.
    /// </summary>
    Undefined = 0,
    
    /// <summary>
    /// Создание вакансии.
    /// </summary>
    CreateVacancy = 1,
    
    /// <summary>
    /// Тариф.
    /// </summary>
    FareRule = 2
}