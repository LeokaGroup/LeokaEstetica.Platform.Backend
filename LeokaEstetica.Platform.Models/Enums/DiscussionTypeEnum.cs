using System.ComponentModel;

namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление типов объектов обсуждения.
/// </summary>
public enum DiscussionTypeEnum
{
    /// <summary>
    /// Тип обсуждения = проект.
    /// </summary>
    [Description("Проект")]
    Project = 1,
    
    /// <summary>
    /// Тип обсуждения = вакансия.
    /// </summary>
    [Description("Вакансия")]
    Vacancy = 2,
    
    /// <summary>
    /// Если вопрос к нейросети.
    /// </summary>
    [Description("Вопрос к нейросети.")]
    ObjectTypeDialogAi = 3
}