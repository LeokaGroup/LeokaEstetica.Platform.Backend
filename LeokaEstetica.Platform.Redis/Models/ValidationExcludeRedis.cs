using ProtoBuf;

namespace LeokaEstetica.Platform.Redis.Models;

/// <summary>
/// Класс модели исключения полей при валидации для работы с Redis.
/// </summary>
[Serializable]
[ProtoContract]
public class ValidationExcludeRedis
{
    /// <summary>
    /// Название параметра, который нужно исключать при валидации.
    /// </summary>
    [ProtoMember(1)]
    public string ParamName { get; set; }
}