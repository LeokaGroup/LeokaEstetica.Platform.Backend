namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement.Output;

/// <summary>
/// Класс результата пространств проектов.
/// </summary>
public class WorkSpaceResult
{
    /// <summary>
    /// Список раб.пространств пользователя (компании пользователя).
    /// </summary>
    public List<WorkSpaceOutput>? UserCompanyWorkSpaces { get; set; }
    
    /// <summary>
    /// Список раб.пространств, где пользователь в участниках (т.е. не его компании).
    /// </summary>
    public List<WorkSpaceOutput>? OtherCompanyWorkSpaces { get; set; }
}