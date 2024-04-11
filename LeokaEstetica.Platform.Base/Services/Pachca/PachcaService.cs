using System.Net.Http.Json;
using System.Text;
using LeokaEstetica.Platform.Base.Abstractions.Services.Pachca;
using LeokaEstetica.Platform.Base.Enums;
using LeokaEstetica.Platform.Base.Models.Input.Pachca;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Base.Services.Pachca;

/// <summary>
/// Класс реализует методы сервиса пачки.
/// </summary>
internal sealed class PachcaService : IPachcaService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<PachcaService> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <param name="logger">Логгер.</param>
    public PachcaService(IConfiguration configuration,
        ILogger<PachcaService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    #region Публичные методы.

     /// <summary>
    /// Метод отправляет уведомлений с деталями ошибки в пачку.
    /// </summary>
    /// <param name="exception">Исключение.</param>
    public async Task SendNotificationErrorAsync(Exception exception)
    {
        try
        {
            string environment = null;

            if (_configuration["Environment"].Equals("Development"))
            {
                environment = "[Develop] ";
            }

            else if (_configuration["Environment"].Equals("Staging"))
            {
                environment = "[Test] ";
            }

            else if (_configuration["Environment"].Equals("Production"))
            {
                environment = "[Production] ";
            }

            var errorMessage = new StringBuilder();
            errorMessage.Append(environment);
            errorMessage.AppendLine("ErrorMessage: ");

            var errMsg = exception.Message;

            errorMessage.AppendLine(errMsg);
            errorMessage.AppendLine("StackTrace: ");
            errorMessage.AppendLine(exception.StackTrace);

            using var httpClient = new HttpClient();
            var request = new SendNotificationInput { Message = errorMessage.ToString() };

            await httpClient.PostAsJsonAsync(_configuration["PachcaBot:AlarmsBot"], request);
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод отправляет уведомление в чат о созданной вакансии, проекте.
    /// </summary>
    /// <param name="objectType">Тип объекта (вакансия, проект).</param>
    /// <param name="objectName">Название объекта (проекта, вакансии).</param>
    public async Task SendNotificationCreatedObjectAsync(ObjectTypeEnum objectType, string objectName)
    {
        var notifyMessage = string.Empty;
        using var httpClient = new HttpClient();

        try
        {
            if (string.IsNullOrWhiteSpace(objectName))
            {
                var ex = new InvalidOperationException(
                    "Название объекта (проекта, вакансии) не заполнено. Невозможно отправить уведомление в канал.");
                throw ex;
            }

            if (objectType.HasFlag(ObjectTypeEnum.Project))
            {
                notifyMessage = $"Создан новый проект \"{objectName}\".";
            }

            else if (objectType.HasFlag(ObjectTypeEnum.Vacancy))
            {
                notifyMessage = $"Создана новая вакансия \"{objectName}\".";
            }

            if (new[] { "Development", "Staging" }.Contains(_configuration["Environment"]))
            {
                await httpClient.PostAsJsonAsync(_configuration["PachcaBot:NotificationsDevelopTestBot"],
                    notifyMessage);
            }

            else
            {
                await httpClient.PostAsJsonAsync(_configuration["PachcaBot:NotificationsBot"], notifyMessage);
            }
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            _logger.LogInformation("Повторная попытка отправить уведомление в чат пачки.");

            try
            {
                if (new[] { "Development", "Staging" }.Contains(_configuration["Environment"]))
                {
                    await httpClient.PostAsJsonAsync(_configuration["PachcaBot:NotificationsDevelopTestBot"],
                        notifyMessage);
                }

                else
                {
                    await httpClient.PostAsJsonAsync(_configuration["PachcaBot:NotificationsBot"], notifyMessage);
                }
            }

            catch (Exception ex2)
            {
                _logger.LogError(ex2, "Повторная отправка уведомления не удалась.");
                throw;
            }
        }
    }

    /// <summary>
    /// Метод отправляет уведомление в пачку о новом пользователе.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    public async Task SendNotificationCreatedNewUserAsync(string account)
    {
        using var httpClient = new HttpClient();
        var notificationInput = new SendNotificationInput
        {
            Message = $"Новый пользователь {account} на платформе!"
        };

        try
        {
            if (new[] { "Development", "Staging" }.Contains(_configuration["Environment"]))
            {
                await httpClient.PostAsJsonAsync(_configuration["PachcaBot:NotificationsDevelopTestBot"],
                    notificationInput);
            }

            else
            {
                await httpClient.PostAsJsonAsync(_configuration["PachcaBot:NotificationsBot"], notificationInput);
            }
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            _logger.LogInformation("Повторная попытка отправить уведомление в чат пачки.");

            try
            {
                if (new[] { "Development", "Staging" }.Contains(_configuration["Environment"]))
                {
                    await httpClient.PostAsJsonAsync(_configuration["PachcaBot:NotificationsDevelopTestBot"],
                        notificationInput);
                }

                else
                {
                    await httpClient.PostAsJsonAsync(_configuration["PachcaBot:NotificationsBot"], notificationInput);
                }
            }

            catch (Exception ex2)
            {
                _logger.LogError(ex2, "Повторная отправка уведомления не удалась.");
                throw;
            }
        }
    }

    /// <summary>
    /// Метод отправляет уведомление в пачку о созданном проекте. Но такой проект еще не прошел модерацию.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    public async Task SendNotificationCreatedProjectBeforeModerationAsync(long projectId)
    {
        using var httpClient = new HttpClient();
        var notificationInput = new SendNotificationInput
        {
            Message = $"Новый проект отправлен на модерацию! Id проекта: {projectId}"
        };

        try
        {
            if (new[] { "Development", "Staging" }.Contains(_configuration["Environment"]))
            {
                await httpClient.PostAsJsonAsync(_configuration["PachcaBot:NotificationsDevelopTestBot"],
                    notificationInput);
            }

            else
            {
                await httpClient.PostAsJsonAsync(_configuration["PachcaBot:NotificationsBot"], notificationInput);
            }
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            _logger.LogInformation("Повторная попытка отправить уведомление в чат пачки.");

            try
            {
                if (new[] { "Development", "Staging" }.Contains(_configuration["Environment"]))
                {
                    await httpClient.PostAsJsonAsync(_configuration["PachcaBot:NotificationsDevelopTestBot"],
                        notificationInput);
                }

                else
                {
                    await httpClient.PostAsJsonAsync(_configuration["PachcaBot:NotificationsBot"], notificationInput);
                }
            }

            catch (Exception ex2)
            {
                _logger.LogError(ex2, "Повторная отправка уведомления не удалась.");
                throw;
            }
        }
    }

    /// <summary>
    /// Метод отправляет уведомление в пачку о созданной вакансии. Но такая вакансия еще не прошла модерацию.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    public async Task SendNotificationCreatedVacancyBeforeModerationAsync(long vacancyId)
    {
        using var httpClient = new HttpClient();
        var notificationInput = new SendNotificationInput
        {
            Message = $"Новая вакансия отправлена на модерацию! Id вакансии: {vacancyId}"
        };

        try
        {
            if (new[] { "Development", "Staging" }.Contains(_configuration["Environment"]))
            {
                await httpClient.PostAsJsonAsync(_configuration["PachcaBot:NotificationsDevelopTestBot"],
                    notificationInput);
            }

            else
            {
                await httpClient.PostAsJsonAsync(_configuration["PachcaBot:NotificationsBot"], notificationInput);
            }
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            _logger.LogInformation("Повторная попытка отправить уведомление в чат пачки.");

            try
            {
                if (new[] { "Development", "Staging" }.Contains(_configuration["Environment"]))
                {
                    await httpClient.PostAsJsonAsync(_configuration["PachcaBot:NotificationsDevelopTestBot"],
                        notificationInput);
                }

                else
                {
                    await httpClient.PostAsJsonAsync(_configuration["PachcaBot:NotificationsBot"], notificationInput);
                }
            }

            catch (Exception ex2)
            {
                _logger.LogError(ex2, "Повторная отправка уведомления не удалась.");
                throw;
            }
        }
    }

    /// <summary>
    /// Метод отправляет уведомление в пачку об изменениях анкеты пользователя. Но такая анкета еще не прошла модерацию.
    /// </summary>
    /// <param name="profileInfoId">Id анкеты.</param>
    public async Task SendNotificationChangedProfileInfoBeforeModerationAsync(long profileInfoId)
    {
        using var httpClient = new HttpClient();
        var notificationInput = new SendNotificationInput
        {
            Message = $"Анкета пользователя изменилась и отправлена на модерацию! Id анкеты: {profileInfoId}"
        };

        try
        {
            if (new[] { "Development", "Staging" }.Contains(_configuration["Environment"]))
            {
                await httpClient.PostAsJsonAsync(_configuration["PachcaBot:NotificationsDevelopTestBot"],
                    notificationInput);
            }

            else
            {
                await httpClient.PostAsJsonAsync(_configuration["PachcaBot:NotificationsBot"], notificationInput);
            }
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            _logger.LogInformation("Повторная попытка отправить уведомление в чат пачки.");

            try
            {
                if (new[] { "Development", "Staging" }.Contains(_configuration["Environment"]))
                {
                    await httpClient.PostAsJsonAsync(_configuration["PachcaBot:NotificationsDevelopTestBot"],
                        notificationInput);
                }

                else
                {
                    await httpClient.PostAsJsonAsync(_configuration["PachcaBot:NotificationsBot"], notificationInput);
                }
            }

            catch (Exception ex2)
            {
                _logger.LogError(ex2, "Повторная отправка уведомления не удалась.");
                throw;
            }
        }
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}