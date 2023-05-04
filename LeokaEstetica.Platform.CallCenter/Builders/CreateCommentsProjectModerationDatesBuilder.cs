using AutoMapper;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Comment;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using System.Globalization;

namespace LeokaEstetica.Platform.CallCenter.Builders;

/// <summary>
/// Билдер строит даты комментариев проекта модерации к нужному виду.
/// </summary>
public static class CreateCommentsProjectModerationDatesBuilder
{
    private static readonly List<CommentProjectModerationOutput> _comments = new();

    /// <summary>
    /// Метод форматирует даты к нужному виду для модерации.
    /// </summary>
    /// <param name="comments">Список коментариев из БД.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <returns>Список с измененными датами.</returns>
    public static IEnumerable<CommentProjectModerationOutput> Create(IEnumerable<ProjectCommentModerationEntity> comments,
        IMapper mapper)
    {
        _comments.Clear();

        foreach (var item in comments)
        {
            // Прежде чем мапить форматируем даты.
            var convertModerationDate = item.DateModeration.ToString("g", CultureInfo.GetCultureInfo("ru"));
            var convertCreatedDate = item.ProjectComment.Created.ToString("g", CultureInfo.GetCultureInfo("ru"));

            // Затем уже мапим к выходной модели.
            var newItem = mapper.Map<CommentProjectModerationOutput>(item);
            newItem.DateModeration = convertModerationDate;
            newItem.DateCreated = convertCreatedDate;
            _comments.Add(newItem);
        }

        return _comments;
    }
}
