using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Database.Abstractions.Project;

namespace LeokaEstetica.Platform.Database.Repositories.ProjectManagment;

/// <summary>
/// Класс реализует методы репозитория ролей управления проектами.
/// </summary>
internal sealed class ProjectManagmentRoleRepository : BaseRepository, IProjectManagmentRoleRepository
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="connectionProvider"></param>
    public ProjectManagmentRoleRepository(IConnectionProvider connectionProvider)
        : base(connectionProvider)
    {
    }
}