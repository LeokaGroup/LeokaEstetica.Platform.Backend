namespace LeokaEstetica.Platform.Models.Dto.Common;

/// <summary>
/// Базовый класс описывающий ошибки.
/// </summary>
public class FrontErrorOutput
{
    /// <summary>
    /// Список ошибок, которые будем выводить на фронт.
    /// </summary>
    public List<string> Errors { get; set; } = new();
}