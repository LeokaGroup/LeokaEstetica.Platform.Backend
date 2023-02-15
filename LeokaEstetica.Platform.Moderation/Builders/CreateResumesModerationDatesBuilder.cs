using System.Globalization;
using AutoMapper;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Resume;
using LeokaEstetica.Platform.Models.Entities.Moderation;

namespace LeokaEstetica.Platform.Moderation.Builders;

/// <summary>
/// Билдер строит даты анкет модерации к нужному виду.
/// </summary>
public static class CreateResumesModerationDatesBuilder
{
    private static readonly List<ResumeModerationOutput> _resumes = new();
    
    /// <summary>
    /// Метод форматирует даты к нужному виду для модерации.
    /// </summary>
    /// <param name="projects">Список проектов из БД.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <returns>Список с измененными датами.</returns>
    public static IEnumerable<ResumeModerationOutput> Create(IEnumerable<ModerationResumeEntity> resumes,
        IMapper mapper)
    {
        _resumes.Clear();
        
        foreach (var item in resumes)
        {
            // Прежде чем мапить форматируем даты.
            var convertModerationDate = item.DateModeration.ToString("g", CultureInfo.GetCultureInfo("ru"));

            // Затем уже мапим к результирующей модели.
            var newItem = mapper.Map<ResumeModerationOutput>(item);
            newItem.DateModeration = convertModerationDate;
            _resumes.Add(newItem);
        }

        return _resumes;
    }
}