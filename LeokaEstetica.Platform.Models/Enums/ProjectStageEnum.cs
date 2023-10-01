using System.ComponentModel;

namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление списка стадий проекта.
/// </summary>
public enum ProjectStageEnum
{
    [Description("Идея")]
    Concept = 1,
    
    [Description("Поиск команды")]
    SearchTeam = 2,
    
    [Description("Проектирование")]
    Projecting = 3,
    
    [Description("Разработка")]
    Development = 4,
    
    [Description("Тестирование")]
    Testing = 5,
    
    [Description("Поиск инвесторов")]
    SearchInvestors = 6,
    
    [Description("Запуск")]
    Start = 7,
    
    [Description("Поддержка")]
    Support = 8
}