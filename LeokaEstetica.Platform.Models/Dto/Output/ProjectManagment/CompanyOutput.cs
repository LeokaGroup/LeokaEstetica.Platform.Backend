namespace LeokaEstetica.Platform.Models.Dto.ProjectManagement;

/// <summary>
/// Класс выходной модели компании.
/// </summary>
public class CompanyOutput
{
    /// <summary>
    /// Id компании.
    /// </summary>
    public long CompanyId { get; set; }

    /// <summary>
    /// Название компании.
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// Id пользователя, который создал компанию.
    /// </summary>
    public long CreatedBy { get; set; }
}