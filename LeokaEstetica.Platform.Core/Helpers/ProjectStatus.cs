using System.ComponentModel;
using LeokaEstetica.Platform.Core.Extensions;

namespace LeokaEstetica.Platform.Core.Helpers;

/// <summary>
/// Класс статусов проектов.
/// </summary>
public static class ProjectStatus
{
    /// <summary>
    /// Словарь статусов с системными именами.
    /// </summary>
    private static readonly IDictionary<string, int> _projectStatusesSysNames = new Dictionary<string, int>()
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
    private static readonly IDictionary<string, string> _projectStatusesNames = new Dictionary<string, string>()
    {
        ["Moderation"] = "На модерации",
        ["Approved"] = "Одобрен",
        ["Rejected"] = "Отклонен",
        ["Draft"] = "В черновике",
        ["Removed"] = "Удален"
    };

    /// <summary>
    /// Метод получает Id статуса по системному названию статуса.
    /// </summary>
    /// <param name="statusName">Системное название статуса.</param>
    /// <returns>Id статуса.</returns>
    public static int GetProjectStatusIdBySysName(string statusName)
    {
        return _projectStatusesSysNames.TryGet(statusName);
    }
    
    /// <summary>
    /// Метод получает русское название статуса по системному названию статуса.
    /// </summary>
    /// <param name="statusName">Системное название статуса.</param>
    /// <returns>Русское название статуса.</returns>
    public static string GetProjectStatusNameBySysName(string statusName)
    {
        return _projectStatusesNames.TryGet(statusName);
    }
}

/// <summary>
/// Перечисление системных названий статусов проектов.
/// </summary>
public enum ProjectStatusNameEnum
{
    [Description("На модерации")]
    Moderation = 1,
    
    [Description("Одобрен")]
    Approved = 2,
    
    [Description("Отклонен")]
    Rejected = 3,
    
    [Description("В черновике")]
    Draft = 4,
    
    [Description("Удален")]
    Removed = 5
}