using LeokaEstetica.Platform.Models.Entities.Communication;
using LeokaEstetica.Platform.Models.Entities.ProjectTeam;

namespace LeokaEstetica.Platform.Models.Entities.Project;

/// <summary>
/// Класс сопоставляется с таблицей каталога проектов Projects.CatalogProjects.
/// </summary>
public class CatalogProjectEntity
{
    public CatalogProjectEntity()
    {
        ProjectsTeams = new HashSet<ProjectTeamEntity>();
        MainInfoDialog = new HashSet<MainInfoDialogEntity>();
    }

    /// <summary>
    /// PK.
    /// </summary>
    public long CatalogProjectId { get; set; }

    public long ProjectId { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public UserProjectEntity Project { get; set; }

    /// <summary>
    /// Список команд проектов.
    /// </summary>
    public ICollection<ProjectTeamEntity> ProjectsTeams { get; set; }

    /// <summary>
    /// Информация о диалоге.
    /// </summary>
    public ICollection<MainInfoDialogEntity> MainInfoDialog { get; set; }
}