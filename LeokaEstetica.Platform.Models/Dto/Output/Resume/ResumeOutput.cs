namespace LeokaEstetica.Platform.Models.Dto.Output.Resume;

/// <summary>
/// Класс выходной модели базы резюме.
/// </summary>
public class ResumeOutput
{
    /// <summary>
    /// Фамилия.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Имя.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Отчество.
    /// </summary>
    public string Patronymic { get; set; }

    /// <summary>
    /// Отображать ли первую букву фамилии.
    /// </summary>
    public bool IsShortFirstName { get; set; }

    /// <summary>
    /// Обо мне.
    /// </summary>
    public string Aboutme { get; set; }

    /// <summary>
    /// Должность.
    /// </summary>
    public string Job { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Код пользователя.
    /// </summary>
    public Guid UserCode { get; set; }

    /// <summary>
    /// Id анкеты пользователя.
    /// </summary>
    public long ProfileInfoId { get; set; }
    
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
}