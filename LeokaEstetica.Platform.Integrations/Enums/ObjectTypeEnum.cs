using System.ComponentModel;

namespace LeokaEstetica.Platform.Integrations.Enums;

/// <summary>
/// Перечисление типов объектов (вакансия, проект).
/// </summary>
[Flags]
public enum ObjectTypeEnum
{
    /// <summary>
    /// Проект.
    /// </summary>
    [Description("Проект")]
    Project = 1,
    
    /// <summary>
    /// Вакансия.
    /// </summary>
    [Description("Вакансия")]
    Vacancy = 2,
}