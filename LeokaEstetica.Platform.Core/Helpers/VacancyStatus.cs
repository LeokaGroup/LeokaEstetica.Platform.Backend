using System.ComponentModel;
using LeokaEstetica.Platform.Core.Extensions;

namespace LeokaEstetica.Platform.Core.Helpers;

/// <summary>
/// Класс статусов вакансий.
/// </summary>
public static class VacancyStatus
{
    /// <summary>
    /// Словарь статусов с системными именами.
    /// </summary>
    private static readonly IDictionary<string, int> _vacancyStatusesSysNames = new Dictionary<string, int>()
    {
        ["Moderation"] = 1,
        ["Approved"] = 2,
        ["Rejected"] = 3,
        ["Draft"] = 4,
        ["Removed"] = 5
    };
    
    /// <summary>
    /// Словарь статусов с русскими именами.
    /// </summary>
    private static readonly IDictionary<string, string> _vacancyStatusesNames = new Dictionary<string, string>()
    {
        ["Moderation"] = "На модерации",
        ["Approved"] = "Одобрена",
        ["Rejected"] = "Отклонена",
        ["Draft"] = "В черновике",
        ["Removed"] = "Удалена"
    };

    /// <summary>
    /// Метод получает Id статуса по системному названию статуса.
    /// </summary>
    /// <param name="statusName">Системное название статуса.</param>
    /// <returns>Id статуса.</returns>
    public static int GetVacancyStatusIdBySysName(string statusName)
    {
        return _vacancyStatusesSysNames.TryGet(statusName);
    }
    
    /// <summary>
    /// Метод получает русское название статуса по системному названию статуса.
    /// </summary>
    /// <param name="statusName">Системное название статуса.</param>
    /// <returns>Русское название статуса.</returns>
    public static string GetVacancyStatusNameBySysName(string statusName)
    {
        return _vacancyStatusesNames.TryGet(statusName);
    }
}

/// <summary>
/// Перечисление системных названий статусов вакансий.
/// </summary>
public enum VacancyStatusNameEnum
{
    [Description("На модерации")]
    Moderation = 1,
    
    [Description("Одобрена")]
    Approved = 2,
    
    [Description("Отклонена")]
    Rejected = 3,
    
    [Description("В черновике")]
    Draft = 4,
    
    [Description("Удалена")]
    Removed = 5
}