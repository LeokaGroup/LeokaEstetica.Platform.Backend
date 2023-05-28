namespace LeokaEstetica.Platform.Models.Dto.Input.Moderation;

/// <summary>
/// Класс входной модели отправки замечаний анкет.
/// </summary>
public class SendResumeRemarkInput
{
    /// <summary>
    /// Id анкеты.
    /// </summary>
    public long ProfileInfoId { get; set; }
}