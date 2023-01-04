using System.ComponentModel;

namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление типов фильтров ключевых слов.
/// </summary>
public enum FilterKeywordsTypeEnum
{
    [Description("В названии вакансии")]
    VacancyName = 1,
    
    [Description("В описании вакансии")]
    VacancyDetail = 2
}