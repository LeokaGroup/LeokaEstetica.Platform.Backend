using LeokaEstetica.Platform.Models.Entities.Profile;

namespace LeokaEstetica.Platform.Models.Dto.Output.Resume;

/// <summary>
/// Класс выходной модели базы резюме.
/// </summary>
public class ResumeOutput : ProfileInfoEntity
{
    /// <summary>
    /// Код пользователя.
    /// </summary>
    public Guid UserCode { get; set; }

    /// <summary>
    /// Признак выделения цветом.
    /// </summary>
    public bool IsSelectedColor { get; set; }
    
    /// <summary>
    /// Цвет тега.
    /// </summary>
    public string TagColor { get; set; }

    /// <summary>
    /// Значение тега.
    /// </summary>
    public string TagValue { get; set; }

    /// <summary>
    /// Почта пользователя.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Телефон пользователя.
    /// </summary>
    public string PhoneNumber { get; set; }
}