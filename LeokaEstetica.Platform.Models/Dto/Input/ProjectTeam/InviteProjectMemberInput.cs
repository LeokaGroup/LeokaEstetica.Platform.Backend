namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectTeam;

/// <summary>
/// Класс входной модели приглашения в команду проекта.
/// </summary>
public class InviteProjectMemberInput
{
    /// <summary>
    /// Список Id пользователей, которые будут добавлены в участники команды проекта.
    /// </summary>
    public List<long> UsersIds { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }
}