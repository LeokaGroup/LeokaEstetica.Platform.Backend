using System.ComponentModel;

namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление типов фильтров стадий проекта.
/// </summary>
public enum FilterProjectStageTypeEnum
{
    [Description("Отсутствует. Не ищем по этому значению.")]
    None = 1,
    
    [Description("Идея.")]
    Concept = 2,
    
    [Description("Поиск команды.")]
    SearchTeam = 3,
    
    [Description("Проектирование.")]
    Projecting = 4,
    
    [Description("Разработка.")]
    Development = 5,
    
    [Description("Разработка.")]
    Testing = 6,
    
    [Description("Поиск инвесторов.")]
    SearchInvestors = 7,
    
    [Description("Запуск.")]
    Start = 8,
    
    [Description("Поддержка.")]
    Support = 9
}