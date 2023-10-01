namespace LeokaEstetica.Platform.Models.Dto.Output.Integration.Telegram;

/// <summary>
/// Класс выходной модели создания ссылки для инвайта в канал уведомлений телеграма.
/// </summary>
public class CreateInviteLInkOutput
{
    /// <summary>
    /// Ссылка на инвайт.
    /// </summary>
    public string Url { get; set; }
}