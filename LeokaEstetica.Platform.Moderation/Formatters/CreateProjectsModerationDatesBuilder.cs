using System.Globalization;
using AutoMapper;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Project;
using LeokaEstetica.Platform.Models.Entities.Moderation;

namespace LeokaEstetica.Platform.Moderation.Formatters;

/// <summary>
/// Билдер строит даты модерации к нужному виду.
/// </summary>
public static class CreateProjectsModerationDatesBuilder
{
    private static List<ProjectModerationOutput> _projects = new();
    
    /// <summary>
    /// Метод форматирует даты к нужному виду для модерации.
    /// </summary>
    /// <param name="projects">Список проектов из БД.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <returns>Список с измененными датами.</returns>
    public static List<ProjectModerationOutput> Create(IEnumerable<ModerationProjectEntity> projects, IMapper mapper)
    {
        _projects.Clear();
        
        foreach (var item in projects)
        {
            var convertModerationDate = item.DateModeration.ToString("g", CultureInfo.GetCultureInfo("ru"));
            var convertCreatedDate = item.UserProject.DateCreated.ToString("g", CultureInfo.GetCultureInfo("ru"));
            var newItem = mapper.Map<ProjectModerationOutput>(item);
            newItem.DateModeration = convertModerationDate;
            newItem.UserProject.DateCreated = convertCreatedDate;
            _projects.Add(newItem);
        }

        return _projects;
    }
}