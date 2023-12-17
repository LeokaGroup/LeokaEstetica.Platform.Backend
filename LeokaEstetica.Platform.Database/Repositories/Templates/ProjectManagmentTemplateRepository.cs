using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Template;

namespace LeokaEstetica.Platform.Database.Repositories.Templates;

/// <summary>
/// Класс реализует методы репозитория шаблонов модуля УП.
/// </summary>
internal sealed class ProjectManagmentTemplateRepository : IProjectManagmentTemplateRepository
{
    private readonly PgContext _pgContext;
    
    /// <summary>
    /// Конструктор.
    /// <param name="pgContext">Датаконтекст.</param>
    /// </summary>
    public ProjectManagmentTemplateRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    /// <summary>
    /// Метод получает список Id статусов, которые принадлежат шаблону.
    /// </summary>
    /// <param name="templateId">Id шаблона.</param>
    /// <returns>Список Id статусов.</returns>
    public Task<IEnumerable<int>> GetTemplateStatusIdsAsync(int templateId)
    {
        throw new NotImplementedException();
    }
}