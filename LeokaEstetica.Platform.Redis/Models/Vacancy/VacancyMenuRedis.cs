using ProtoBuf;

namespace LeokaEstetica.Platform.Redis.Models.Vacancy;

/// <summary>
/// Класс модели для хранения меню вакансий для работы с кэшем Redis.
/// </summary>
[ProtoContract]
public class VacancyMenuRedis
{
    /// <summary>
    /// Элементы меню вакансий.
    /// </summary>
    [ProtoMember(1)]
    public List<VacancyMenuItemsRedis> VacancyMenuItems { get; set; }
}