using System.Globalization;
using AutoMapper;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Entities.Communication;

namespace LeokaEstetica.Platform.Messaging.Builders;

/// <summary>
/// Билдер строит даты комментариев проектов к нужному виду.
/// </summary>
public static class CreateProjectCommentsDatesBuilder
{
    private static readonly List<ProjectCommentOutput> _projectComments = new();
    
    /// <summary>
    /// Метод форматирует даты к нужному виду для модерации.
    /// </summary>
    /// <param name="projects">Список проектов из БД.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <returns>Список с измененными датами.</returns>
    public static IEnumerable<ProjectCommentOutput> Create(IEnumerable<ProjectCommentEntity> projects, IMapper mapper)
    {
        _projectComments.Clear();
        
        foreach (var item in projects)
        {
            // Затем уже мапим к результирующей модели.
            var newItem = mapper.Map<ProjectCommentOutput>(item);
            newItem.Created = item.Created.ToString("g", CultureInfo.GetCultureInfo("ru"));;
            _projectComments.Add(newItem);
        }

        return _projectComments;
    }
}