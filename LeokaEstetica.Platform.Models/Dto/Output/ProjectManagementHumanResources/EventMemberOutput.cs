using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Models.Extensions;
using Enum = LeokaEstetica.Platform.Models.Enums.Enum;

namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagementHumanResources;

/// <summary>
/// Класс выходной модели участников события.
/// </summary>
public class EventMemberOutput
{
    /// <summary>
    /// PK.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Id участника события.
    /// </summary>
    public long EventMemberId { get; set; }

    /// <summary>
    /// Id события.
    /// </summary>
    public long EventId { get; set; }

    /// <summary>
    /// Значение енамки статуса участника события.
    /// </summary>
    public CalendarEventMemberStatusEnum CalendarEventMemberStatusValue { get; set; }

    /// <summary>
    /// Статус участника события.
    /// </summary>
    public IEnum CalendarEventMemberStatus
    {
        get => new Enum(CalendarEventMemberStatusValue);
        set => CalendarEventMemberStatusValue = Enum.FromString<CalendarEventMemberStatusEnum>(
            value.Value.ToPascalCaseFromSnakeCase());
    }

    /// <summary>
    /// Дата присоединения к событию.
    /// </summary>
    public DateTime Joined { get; set; }

    /// <summary>
    /// Почта пользователя.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Название статуса для отображения.
    /// </summary>
    public string? DisplayEventMemberStatus { get; set; }
}