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
    /// Оформление тарифа.
    /// </summary>
    FareRule = 1,
    
    /// <summary>
    /// Оплата публикации вакансии.
    /// </summary>
    CreateVacancy = 2,
    
    /// <summary>
    /// Оплата открытия контакта в базе резюме.
    /// </summary>
    OpenResume = 3
}