using System.ComponentModel;

namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление типов фильтров ключевых слов.
/// </summary>
public enum FilterKeywordsTypeEnum
{
    [Description("Отсутствует. Не ищем по этому значению.")]
    None = 1,
    
    [Description("В названии вакансии")]
    VacancyName = 2,
    
    [Description("В описании вакансии")]
    VacancyDetail = 3
}