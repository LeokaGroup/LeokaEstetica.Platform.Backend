namespace LeokaEstetica.Platform.Models.Dto.ProjectManagement;

/// <summary>
/// Класс выходной модели проверки кол-ва компаний пользователя.
/// </summary>
public class CalculateUserCompanyOutput
{
    /// <summary>
    /// Признак наличия минимум 1 компании у пользователя.
    /// </summary>
    public bool IfExistsAnyCompanies { get; set; }

    /// <summary>
    /// Признак наличия более 1 компании у пользователя.
    /// </summary>
    public bool IfExistsMultiCompanies { get; set; }

    /// <summary>
    /// Признак необходимости решения от пользователя.
    /// </summary>
    public bool IsNeedUserAction { get; set; }
}