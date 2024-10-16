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
    public string? Label { get; set; }
    
    /// <summary>
    /// Тип группы абстрактных областей.
    /// </summary>
    public AbstractGroupTypeEnum AbstractGroupType { get; set; }

    /// <summary>
    /// Id пользователя области.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Признак наличия у объекта его диалогов.
    /// </summary>
    public bool HasDialogs { get; set; }

    /// <summary>
    /// Вложенные элементы.
    /// </summary>
    public List<GroupObjectDialogMessageOutput>? Items { get; set; }
}