namespace LeokaEstetica.Platform.Models.Dto.Input.Notification;

/// <summary>
/// Класс входной модели для фиксации ConnectionId в кэше.
/// </summary>
public class CommitConnectionInput
{
    /// <summary>
    /// Id подключения.
    /// </summary>
    public string ConnectionId { get; set; }
}