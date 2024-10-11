using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Models.Dto.Communications.Output;

/// <summary>
/// Класс выходной модели абстрактной области чата.
/// </summary>
public class AbstractScopeOutput
{
    /// <summary>
    /// Название абстрактной области.
    /// </summary>
    public string? ScopeName { get; set; }

    /// <summary>
    /// Тип абстрактных областей.
    /// </summary>
    public AbstractScopeTypeEnum AbstractScopeType { get; set; }

    /// <summary>
    /// Id пользователя области.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Id абстрактной области.
    /// </summary>
    public long? AbstractScopeId { get; set; }
}