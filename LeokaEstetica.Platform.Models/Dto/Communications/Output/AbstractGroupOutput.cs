using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Models.Dto.Communications.Output;

/// <summary>
/// Класс выходной модели групп абстрактной области чата.
/// </summary>
public class AbstractGroupOutput
{
    /// <summary>
    /// Id группы абстрактной области.
    /// </summary>
    public long AbstractGroupId { get; set; }

    /// <summary>
    /// Название группы абстрактной области.
    /// </summary>
    public string? AbstractGroupName { get; set; }
    
    /// <summary>
    /// Тип группы абстрактных областей.
    /// </summary>
    public AbstractGroupTypeEnum AbstractGroupType { get; set; }

    /// <summary>
    /// Id пользователя области.
    /// </summary>
    public long UserId { get; set; }
}