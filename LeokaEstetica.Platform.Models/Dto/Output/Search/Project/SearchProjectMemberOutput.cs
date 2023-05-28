namespace LeokaEstetica.Platform.Models.Dto.Output.Search.Project;

/// <summary>
/// Класс выходной модели поиска участников проекта.
/// </summary>
public class SearchProjectMemberOutput
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