using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement;

namespace LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;

/// <summary>
/// Абстракция сервиса дерева Wiki модуля УП.
/// </summary>
public interface IWikiTreeService
{
    /// <summary>
    /// Метод получает дерево.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Дерево с вложенными элементами.</returns>
    Task<IEnumerable<WikiTreeFolderItem>> GetTreeAsync(long projectId);
}