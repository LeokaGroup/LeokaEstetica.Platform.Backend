using System.Text.Json.Serialization;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.User;

namespace LeokaEstetica.Platform.Models.Entities.Communication;

/// <summary>
/// Класс сопоставляется с таблицей комментариев к проектам Communications.ProjectComments.
/// TODO: колонку ModerationStatusId в базе можно в принципе удалить, но потом протестировать все
/// TODO: таблица ссылается на "Moderation"."ProjectCommentsModeration" в которой уже есть ModerationStatusId
/// </summary>
public class ProjectCommentEntity
{
    public ProjectCommentEntity()
    {
        ProjectCommentsModeration = new HashSet<ProjectCommentModerationEntity>();
    }

    /// <summary>
    /// PK.
    /// </summary>
    public long CommentId { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Текст комментария.
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// Признак принадлежности комментария текущему пользователю.
    /// </summary>
    public bool IsMyComment { get; set; }

    /// <summary>
    /// Дата создания комментария.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }
    
    /// <summary>
    /// Id статуса модерации.
    /// </summary>
    public int ModerationStatusId { get; set; }

    /// <summary>
    /// Список комментариев на модерации.
    /// </summary>
    [JsonIgnore]
    public ICollection<ProjectCommentModerationEntity> ProjectCommentsModeration { get; set; }

    /// <summary>
    /// FK.
    /// </summary>
    [JsonIgnore]
    public UserEntity User { get; set; }
    
    /// <summary>
    /// FK.
    /// </summary>
    public ModerationStatusEntity ModerationStatus { get; set; }
}