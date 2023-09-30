using LeokaEstetica.Platform.Integrations.Enums;

namespace LeokaEstetica.Platform.Integrations.Abstractions.Pachca;

/// <summary>
/// Абстракция сервиса пачки.
/// </summary>
public interface IPachcaService
{
    /// <summary>
    /// Метод отправляет уведомлений с деталями ошибки в пачку.
    /// </summary>
    /// <param name="exception">Исключение.</param>
    Task SendNotificationErrorAsync(Exception exception);
    
    /// <summary>
    /// Метод отправляет уведомление в пачку о созданной вакансии, проекте.
    /// </summary>
    /// <param name="objectType">Тип объекта (вакансия, проект).</param>
    /// <param name="objectName">Название объекта (проекта, вакансии).</param>
    Task SendNotificationCreatedObjectAsync(ObjectTypeEnum objectType, string objectName);
}