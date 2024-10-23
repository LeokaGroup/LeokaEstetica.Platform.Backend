namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление типа результата классификатора.
/// Перечисляет варианты результата работы одного из классификаторов.
/// </summary>
public enum ClassifierResultTypeEnum
{
    /// <summary>
    /// Неизвестный тип.
    /// </summary>
    Undefined = 0,
    
    /// <summary>
    /// Проект.
    /// </summary>
    Project = 1,
    
    /// <summary>
    /// Компания.
    /// </summary>
    Company = 2
}