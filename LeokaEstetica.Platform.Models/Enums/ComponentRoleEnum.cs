namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление компонентных ролей.
/// </summary>
public enum ComponentRoleEnum
{
    /// <summary>
    /// Неизвестный тип.
    /// </summary>
    Undefined = 0,
    
    /// <summary>
    /// Соискатель.
    /// </summary>
    JobSeeker = 1,
    
    /// <summary>
    /// Владелец проекта.
    /// </summary>
    Owner = 2,
    
    /// <summary>
    /// Кадровое или аутстаффинговое агентство.
    /// </summary>
    RecruitmentOrOutsourcingAgency = 3
}