using ProtoBuf;

namespace LeokaEstetica.Platform.Redis.Models.ProjectManagement;

/// <summary>
/// Класс модели ролей модуля УП для хранения в кэше Redis.
/// </summary>
[ProtoContract]
public class ProjectManagementRoleRedis
{
    /// <summary>
    /// Id компании.
    /// </summary>
    [ProtoMember(1)]
    public long OrganizationId { get; set; }

    /// <summary>
    /// Id участника компании.
    /// </summary>
    [ProtoMember(2)]
    public long OrganizationMemberId { get; set; }

    /// <summary>
    /// Название роли.
    /// </summary>
    [ProtoMember(3)]
    public string? RoleName { get; set; }

    /// <summary>
    /// Системное название роли.
    /// </summary>
    [ProtoMember(4)]
    public string? RoleSysName { get; set; }

    /// <summary>
    /// Признак активной роли.
    /// </summary>
    [ProtoMember(5)]
    public bool IsActive { get; set; }

    /// <summary>
    /// Признак активной роли у участника проекта компании.
    /// </summary>
    [ProtoMember(6)]
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Id проекта компании.
    /// </summary>
    [ProtoMember(7)]
    public long ProjectId { get; set; }
}