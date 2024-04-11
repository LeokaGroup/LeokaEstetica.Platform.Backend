using System.Net.Http.Json;
using System.Text;
using LeokaEstetica.Platform.Base.Enums;
using LeokaEstetica.Platform.Base.Models.Input.Pachca;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Integrations.Services.Discord;

/// <summary>
/// Класс реализует методы сервиса дискорда.
/// </summary>
internal sealed class DiscordService : IDiscordService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DiscordService> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <param name="logger">Логгер.</param>
    public DiscordService(IConfiguration configuration,
        ILogger<DiscordService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }
    
    /// <inheritdoc />
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
            errorMessage.AppendLine("ErrorMessage: ");

            var errMsg = exception.Message;

            errorMessage.AppendLine(errMsg);
            errorMessage.AppendLine("StackTrace: ");
            errorMessage.AppendLine(exception.StackTrace);

            using var httpClient = new HttpClient();
            var request = new SendNotificationInput
            {
                Embeds = new List<EmbedsItem>
                {
                    new()
                    {
                        Title = environment,
                        Description = errorMessage.ToString()
                    }
                }
            };

            await httpClient.PostAsJsonAsync(_configuration["DiscordBot:AlarmsBot"], request);
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task SendNotificationCreatedObjectAsync(ObjectTypeEnum objectType, string objectName)
    {
        var notifyMessage = string.Empty;
        using var httpClient = new HttpClient();
        
        var request = new SendNotificationInput { Embeds = new List<EmbedsItem>() };

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
            
            request.Embeds.Append(new EmbedsItem { Description = notifyMessage });

            if (new[] { "Development", "Staging" }.Contains(_configuration["Environment"]))
            {
                await httpClient.PostAsJsonAsync(_configuration["DiscordBot:NotificationsDevelopTestBot"], request);
            }

            else
            {
                await httpClient.PostAsJsonAsync(_configuration["DiscordBot:NotificationsBot"], request);
            }
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            _logger.LogInformation("Повторная попытка отправить уведомление в чат дискорда.");

            try
            {
                if (new[] { "Development", "Staging" }.Contains(_configuration["Environment"]))
                {
                    await httpClient.PostAsJsonAsync(_configuration["DiscordBot:NotificationsDevelopTestBot"],
                        request);
                }

                else
                {
                    await httpClient.PostAsJsonAsync(_configuration["DiscordBot:NotificationsBot"], request);
                }
            }

            catch (Exception ex2)
            {
                _logger.LogError(ex2, "Повторная отправка уведомления не удалась.");
                throw;
            }
        }
    }

    /// <inheritdoc />
    public async Task SendNotificationCreatedNewUserAsync(string account)
    {
        using var httpClient = new HttpClient();
        var request = new SendNotificationInput
        {
            Embeds = new List<EmbedsItem>
            {
                new()
                {
                    Description = $"Новый пользователь {account} на платформе!"
                }
            }
        };

        try
        {
            if (new[] { "Development", "Staging" }.Contains(_configuration["Environment"]))
            {
                await httpClient.PostAsJsonAsync(_configuration["DiscordBot:NotificationsDevelopTestBot"],
                    request);
            }

            else
            {
                await httpClient.PostAsJsonAsync(_configuration["DiscordBot:NotificationsBot"], request);
            }
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            _logger.LogInformation("Повторная попытка отправить уведомление в чат дискорда.");

            try
            {
                if (new[] { "Development", "Staging" }.Contains(_configuration["Environment"]))
                {
                    await httpClient.PostAsJsonAsync(_configuration["DiscordBot:NotificationsDevelopTestBot"],
                        request);
                }

                else
                {
                    await httpClient.PostAsJsonAsync(_configuration["DiscordBot:NotificationsBot"], request);
                }
            }

            catch (Exception ex2)
            {
                _logger.LogError(ex2, "Повторная отправка уведомления не удалась.");
                throw;
            }
        }
    }

    /// <inheritdoc />
    public async Task SendNotificationCreatedProjectBeforeModerationAsync(long projectId)
    {
        using var httpClient = new HttpClient();
        var request = new SendNotificationInput
        {
            Embeds = new List<EmbedsItem>
            {
                new()
                {
                    Description = $"Новый проект отправлен на модерацию! Id проекта: {projectId}"
                }
            }
        };

        try
        {
            if (new[] { "Development", "Staging" }.Contains(_configuration["Environment"]))
            {
                await httpClient.PostAsJsonAsync(_configuration["DiscordBot:NotificationsDevelopTestBot"],
                    request);
            }

            else
            {
                await httpClient.PostAsJsonAsync(_configuration["DiscordBot:NotificationsBot"], request);
            }
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            _logger.LogInformation("Повторная попытка отправить уведомление в чат дискорда.");

            try
            {
                if (new[] { "Development", "Staging" }.Contains(_configuration["Environment"]))
                {
                    await httpClient.PostAsJsonAsync(_configuration["DiscordBot:NotificationsDevelopTestBot"],
                        request);
                }

                else
                {
                    await httpClient.PostAsJsonAsync(_configuration["DiscordBot:NotificationsBot"], request);
                }
            }

            catch (Exception ex2)
            {
                _logger.LogError(ex2, "Повторная отправка уведомления не удалась.");
                throw;
            }
        }
    }

    /// <inheritdoc />
    public async Task SendNotificationCreatedVacancyBeforeModerationAsync(long vacancyId)
    {
        using var httpClient = new HttpClient();
        var request = new SendNotificationInput
        {
            Embeds = new List<EmbedsItem>
            {
                new()
                {
                    Description = $"Новая вакансия отправлена на модерацию! Id вакансии: {vacancyId}"
                }
            }
        };

        try
        {
            if (new[] { "Development", "Staging" }.Contains(_configuration["Environment"]))
            {
                await httpClient.PostAsJsonAsync(_configuration["DiscordBot:NotificationsDevelopTestBot"],
                    request);
            }

            else
            {
                await httpClient.PostAsJsonAsync(_configuration["DiscordBot:NotificationsBot"], request);
            }
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            _logger.LogInformation("Повторная попытка отправить уведомление в чат дискорда.");

            try
            {
                if (new[] { "Development", "Staging" }.Contains(_configuration["Environment"]))
                {
                    await httpClient.PostAsJsonAsync(_configuration["DiscordBot:NotificationsDevelopTestBot"],
                        request);
                }

                else
                {
                    await httpClient.PostAsJsonAsync(_configuration["DiscordBot:NotificationsBot"], request);
                }
            }

            catch (Exception ex2)
            {
                _logger.LogError(ex2, "Повторная отправка уведомления не удалась.");
                throw;
            }
        }
    }

    /// <inheritdoc />
    public async Task SendNotificationChangedProfileInfoBeforeModerationAsync(long profileInfoId)
    {
        using var httpClient = new HttpClient();
        var request = new SendNotificationInput
        {
            Embeds = new List<EmbedsItem>
            {
                new()
                {
                    Description = "Анкета пользователя изменилась и отправлена на модерацию! " +
                                  $"Id анкеты: {profileInfoId}"
                }
            }
        };

        try
        {
            if (new[] { "Development", "Staging" }.Contains(_configuration["Environment"]))
            {
                await httpClient.PostAsJsonAsync(_configuration["DiscordBot:NotificationsDevelopTestBot"],
                    request);
            }

            else
            {
                await httpClient.PostAsJsonAsync(_configuration["DiscordBot:NotificationsBot"], request);
            }
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            _logger.LogInformation("Повторная попытка отправить уведомление в чат дискорда.");

            try
            {
                if (new[] { "Development", "Staging" }.Contains(_configuration["Environment"]))
                {
                    await httpClient.PostAsJsonAsync(_configuration["DiscordBot:NotificationsDevelopTestBot"],
                        request);
                }

                else
                {
                    await httpClient.PostAsJsonAsync(_configuration["DiscordBot:NotificationsBot"], request);
                }
            }

            catch (Exception ex2)
            {
                _logger.LogError(ex2, "Повторная отправка уведомления не удалась.");
                throw;
            }
        }
    }
}