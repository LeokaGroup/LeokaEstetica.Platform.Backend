using ProtoBuf;

namespace LeokaEstetica.Platform.Redis.Models;

/// <summary>
/// Класс модели исключения полей при валидации для работы с Redis.
/// </summary>
[ProtoContract]
public class ValidationExcludeRedis
{
    /// <summary>
    /// PK.
    /// </summary>
    [ProtoMember(1)]
    public int ValidationId { get; set; }

    /// <summary>
    /// Название параметра, который нужно исключать при валидации.
    /// </summary>
    [ProtoMember(2)]
    public string ParamName { get; set; }
}