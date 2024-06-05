using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Services.Services.ProjectManagment;

/// <summary>
/// Класс реализует методы сервиса ролей управления проектами.
/// </summary>
internal sealed class ProjectManagmentRoleService : IProjectManagmentRoleService
{
    private readonly ILogger<ProjectManagmentRoleService> _logger;
    private readonly Lazy<IProjectManagmentRoleRepository> _projectManagmentRoleRepository;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="projectManagmentRoleRepository">Репозиторий ролей.</param>
    public ProjectManagmentRoleService(ILogger<ProjectManagmentRoleService> logger,
     Lazy<IProjectManagmentRoleRepository> projectManagmentRoleRepository)
    {
        _logger = logger;
        _projectManagmentRoleRepository = projectManagmentRoleRepository;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProjectManagementRoleOutput>> GetUserRolesAsync(long? projectId = null)
    {
        
    }
}