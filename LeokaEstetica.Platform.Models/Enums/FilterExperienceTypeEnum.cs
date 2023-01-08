using System.ComponentModel;

namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление типов фильтров опыта работы.
/// </summary>
public enum FilterExperienceTypeEnum
{
    [Description("Отсутствует. Не ищем по этому значению.")]
    None = 1,
    
    [Description("Не имеет значения")]
    Unknown = 2,
    
    [Description("От 3 до 6 лет")]
    ThreeSix = 3,
    
    [Description("Более 6 лет")]
    ManySix = 4,
    
    [Description("От 1 года до 3 лет")]
    OneThree = 5,
    
    [Description("Нет опыта")]
    NotExperience = 6
}