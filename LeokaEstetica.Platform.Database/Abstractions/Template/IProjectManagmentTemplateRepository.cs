namespace LeokaEstetica.Platform.Database.Abstractions.Template;

/// <summary>
/// Абстракция шаблонов модуля УП.
/// </summary>
public interface IProjectManagmentTemplateRepository
{
    /// <summary>
    /// Метод получает список Id статусов, которые принадлежат шаблону.
    /// </summary>
    /// <param name="templateId">Id шаблона.</param>
    /// <returns>Список Id статусов.</returns>
    Task<IEnumerable<int>> GetTemplateStatusIdsAsync(int templateId);
}