using System.ComponentModel;

namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление типов фильтров проектов с вакансиями.
/// </summary>
public enum FilterProjectVacancyTypeEnum
{
    [Description("Отсутствует. Не ищем по этому значению.")]
    None = 1,
    
    [Description("Проект имеет вакансии.")]
    Any = 2,
    
    [Description("Проект не имеет вакансий.")]
    NotAny = 3
}