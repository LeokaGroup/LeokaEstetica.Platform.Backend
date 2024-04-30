namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectTeam;

/// <summary>
/// Класс входной модели роли участника команды проекта.
/// </summary>
public class TeamMemberRoleInput
{
    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Название роли.
    /// </summary>
    public string? Role { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }
}