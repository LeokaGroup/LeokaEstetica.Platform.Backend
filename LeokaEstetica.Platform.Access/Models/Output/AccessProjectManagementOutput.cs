namespace LeokaEstetica.Platform.Access.Models.Output;

/// <summary>
/// TODO: Дублируется с <see cref="InviteProjectTeamOutput"/>. Придумать, как их обобщить в одном слое.
/// Класс выходной модели проверки доступа к модулям платформы.
/// </summary>
public class AccessProjectManagementOutput
{
    /// <summary>
    /// Признак наличия доступа к модулю УП.
    /// </summary>
    public bool IsAccess { get; set; }

    /// <summary>
    /// Заголовок запрета.
    /// </summary>
    public string? ForbiddenTitle { get; set; }
    
    /// <summary>
    /// Описание запрета.
    /// </summary>
    public string? ForbiddenText { get; set; }

    /// <summary>
    /// Информация о тарифах, на которых доступна функция.
    /// </summary>
    public string? FareRuleText { get; set; }
}