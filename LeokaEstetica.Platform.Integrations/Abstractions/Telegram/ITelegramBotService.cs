using LeokaEstetica.Platform.Base.Enums;

namespace LeokaEstetica.Platform.Integrations.Abstractions.Telegram;

/// <summary>
/// Абстракция сервиса телеграм бота.
/// </summary>
public interface ITelegramBotService
{
    /// <summary>
    /// Метод отправляет информацию об ошибке в канал телеграма.
    /// </summary>
    /// <param name="errorMessage">Вся инфолрмация об исключении.</param>
    Task SendErrorMessageAsync(string errorMessage);

    /// <summary>
    /// Метод отправляет уведомление в чат о созданной вакансии, проекте.
    /// </summary>
    /// <param name="objectType">Тип объекта (вакансия, проект).</param>
    /// <param name="objectName">Название объекта (проекта, вакансии).</param>
    /// <param name="objectDescription">Описание объекта (проекта, вакансии).</param>
    /// <param name="objectId">Id объекта (проекта, вакансии).</param>
    Task SendNotificationCreatedObjectAsync(ObjectTypeEnum objectType, string objectName, string objectDescription,
        long objectId);
}