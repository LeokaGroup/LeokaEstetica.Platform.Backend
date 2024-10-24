namespace LeokaEstetica.Platform.Models.Dto.Communications.Input;

/// <summary>
/// Класс входной модели создания личного диалога.
/// </summary>
public class CreatePersonalDialogInput
{
    /// <summary>
    /// Почта пользователя, с которым текущий пользователь создает ЛС.
    /// </summary>
    public string? MemberMail { get; set; }
}