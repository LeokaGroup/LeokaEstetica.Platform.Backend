using LeokaEstetica.Platform.Base.Enums;

namespace LeokaEstetica.Platform.Base.Abstractions.Services.Pachca;

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

    /// <summary>
    /// Метод отправляет уведомление в пачку о новом пользователе.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    Task SendNotificationCreatedNewUserAsync(string account);
}