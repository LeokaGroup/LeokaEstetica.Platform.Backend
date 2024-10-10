namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagementHumanResources;

/// <summary>
/// Класс входной модели участников события.
/// </summary>
public class EventMemberInput
{
    /// <summary>
    /// Id участника события.
    /// </summary>
    public long? EventMemberId { get; set; }
    
    /// <summary>
    /// Почта участника события.
    /// </summary>
    public string EventMemberMail { get; set; }
}