using System.ComponentModel;

namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление типов фильтров опыта работы.
/// </summary>
public enum FilterExperienceTypeEnum
{
    [Description("Не имеет значения")]
    Unknown = 1,
    
    [Description("От 3 до 6 лет")]
    ThreeSix = 2,
    
    [Description("Более 6 лет")]
    ManySix = 3,
    
    [Description("От 1 года до 3 лет")]
    OneThree = 4,
    
    [Description("Нет опыта")]
    NotExperience = 5
}