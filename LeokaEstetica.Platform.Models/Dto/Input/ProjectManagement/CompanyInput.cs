using LeokaEstetica.Platform.Models.Dto.Base.ProjectManagement;

namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели компании.
/// </summary>
public class CompanyInput : BaseCompany
{
    /// <summary>
    /// Название компании.
    /// </summary>
    public string? CompanyName { get; set; }
}