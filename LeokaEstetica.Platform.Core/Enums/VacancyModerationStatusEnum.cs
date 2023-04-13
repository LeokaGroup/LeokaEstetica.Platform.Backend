using System.ComponentModel;

namespace LeokaEstetica.Platform.Core.Enums;

/// <summary>
/// Перечисление статусов модерации вакансий.
/// </summary>
public enum VacancyModerationStatusEnum
{
    [Description("Одобрена")]
    ApproveVacancy = 1,
    
    [Description("На модерации")]
    ModerationVacancy = 2,
    
    [Description("Отклонена")]
    RejectedVacancy = 3,

    [Description("В архиве")]
    ArchivedVacancy = 6,
}