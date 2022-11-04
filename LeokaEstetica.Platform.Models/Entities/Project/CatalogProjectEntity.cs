namespace LeokaEstetica.Platform.Models.Entities.Project;

/// <summary>
/// Класс сопоставляется с таблицей каталога проектов Projects.CatalogProjects.
/// </summary>
public class CatalogProjectEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long CatalogProjectId { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public UserProjectEntity ProjectId { get; set; }
}