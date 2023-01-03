namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectTeam;

/// <summary>
/// Класс выходной модели приглашения в команду проекта.
/// </summary>
public class InviteProjectMemberOutput
{
    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Отображаемое имя (почта или логин, смотря что заполнено).
    /// </summary>
    public string DisplayName { get; set; }
}