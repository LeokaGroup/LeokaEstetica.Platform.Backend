using ProtoBuf;

namespace LeokaEstetica.Platform.Models.Dto.Common.Cache;

/// <summary>
/// Класс модели компании в кэше.
/// </summary>
[ProtoContract]
public class CompanyRedis
{
    /// <summary>
    /// Название компании.
    /// </summary>
    [ProtoMember(1)]
    public string? CompanyName { get; set; }
    
    /// <summary>
    /// Id пользователя создавшего компанию.
    /// </summary>
    [ProtoMember(2)]
    public long CreatedBy { get; set; }
}