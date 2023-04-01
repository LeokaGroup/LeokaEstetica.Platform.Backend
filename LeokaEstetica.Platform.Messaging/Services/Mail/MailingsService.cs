using System.Net;
using System.Text;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Messaging.Abstractions.Mail;
using LeokaEstetica.Platform.Messaging.Models.Mail.Input.Mailopost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Messaging.Services.Mail;

/// <summary>
/// Класс реализует методы сервиса работы с сообщениями почты.
/// </summary>
public class MailingsService : IMailingsService
{
    private readonly IConfiguration _configuration;
    private readonly IGlobalConfigRepository _globalConfigRepository;

    public MailingsService(IConfiguration configuration, 
        IGlobalConfigRepository globalConfigRepository)
    {
        _configuration = configuration;
        _globalConfigRepository = globalConfigRepository;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод отправит подтверждение на почту.
    /// </summary>
    /// <param name="mailTo">Email кому отправить.</param>
    /// <param name="confirmEmailCode">Код подтверждения почты.</param>
    public async Task SendConfirmEmailAsync(string mailTo, Guid confirmEmailCode)
    {
        // TODO: Заменить на получение ссылки из БД.
        var html = "Рады вас видеть на Leoka Estetica!" +
                   "<br/><br/>" +
                   $"Для завершения регистрации перейдите по ссылке <a href='https://leoka-estetica-dev.ru/user/account/confirm?code={confirmEmailCode}'>Подтвердить почту</a>" +
                   "<br/>-----<br/>" +
                   "С уважением, команда Leoka Estetica";
        var subject = "Активация аккаунта на leoka-estetica.ru";
        var text = "Рады вас видеть на Leoka Estetica!" +
                   "<br/><br/>" +
                   $"Для завершения регистрации перейдите по ссылке <a href='https://leoka-estetica-dev.ru/user/confirm-email?code={confirmEmailCode}'>Подтвердить почту</a>" +
                   "<br/>-----<br/>" +
                   "С уважением, команда Leoka Estetica";

        var mailModel = CreateMailopostModelConfirmEmail(mailTo, html, subject, text);
        await SendEmailNotificationAsync(mailModel);
    }

    /// <summary>
    /// Метод отправляет уведомление на почту владельца проекта о создании проекта.
    /// Указывается вакансия, если она заполнена.
    /// </summary>
    /// <param name="mailTo">Почта владельца проекта.</param>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    public async Task SendNotificationCreatedProjectAsync(string mailTo, string projectName, long projectId)
    {
        var isEnabledEmailNotifications = await _globalConfigRepository
            .GetValueByKeyAsync<bool>(GlobalConfigKeys.EmailNotifications.EMAIL_NOTIFICATIONS_DISABLE_MODE_ENABLED);

        if (isEnabledEmailNotifications)
        {
            // TODO: Заменить на получение ссылки из БД.
            var text = $"<a href='https://leoka-estetica-dev.ru/projects/project?projectId={projectId}&mode=view'>" +
                       "Перейти к проекту" +
                       "</a>";
            
            var html = $"Вы создали новый проект: \"{projectName}\"" +
                       "<br/>" +
                       text +
                       "<br/>" +
                       "<br/>" +
                       "<br/>" +
                       "<br/>-----<br/>" +
                       "С уважением, команда Leoka Estetica";
            var subject = $"Создан новый проект: \"{projectName}\"";

            var mailModel = CreateMailopostModelConfirmEmail(mailTo, html, subject, text);
            await SendEmailNotificationAsync(mailModel);
        }
    }

    /// <summary>
    /// Метод отправляет уведомление на почту. владельца вакансии о создании новой вакансии.
    /// </summary>
    /// <param name="mailTo">Почта владельца проекта.</param>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    public async Task SendNotificationCreateVacancyAsync(string mailTo, string vacancyName, long vacancyId)
    {
        var isEnabledEmailNotifications = await _globalConfigRepository
            .GetValueByKeyAsync<bool>(GlobalConfigKeys.EmailNotifications.EMAIL_NOTIFICATIONS_DISABLE_MODE_ENABLED);

        if (isEnabledEmailNotifications)
        {
            // TODO: Заменить на получение ссылки из БД.
            var text = $"<a href='https://leoka-estetica-dev.ru/vacancies/vacancy?vacancyId={vacancyId}&mode=view'>" +
                       "Перейти к вакансии" +
                       "</a>";
            
            var html = $"Вы создали новую вакансию: \"{vacancyName}\"" +
                       "<br/>" +
                       text +
                       "<br/>" +
                       "<br/>" +
                       "<br/>" +
                       "<br/>-----<br/>" +
                       "С уважением, команда Leoka Estetica";
            var subject = $"Создана новая вакансия: \"{vacancyName}\"";

            var mailModel = CreateMailopostModelConfirmEmail(mailTo, html, subject, text);
            await SendEmailNotificationAsync(mailModel);
        }
    }

    /// <summary>
    /// Метод отправляет уведомление на почту владельца вакансии об удалении вакансии.
    /// </summary>
    /// <param name="mailTo">Почта владельца проекта.</param>
    /// <param name="vacancyName">Название вакансии.</param>
    public async Task SendNotificationDeleteVacancyAsync(string mailTo, string vacancyName)
    {
        var isEnabledEmailNotifications = await _globalConfigRepository
            .GetValueByKeyAsync<bool>(GlobalConfigKeys.EmailNotifications.EMAIL_NOTIFICATIONS_DISABLE_MODE_ENABLED);

        if (isEnabledEmailNotifications)
        {
            // TODO: Заменить на получение ссылки из БД.
            var text = $"Вы удалили вакансию: \"{vacancyName}\"." +
                       "<br/>" +
                       "<br/>" +
                       "<br/>" +
                       "<br/>-----<br/>" +
                       "С уважением, команда Leoka Estetica";
            var subject = $"Удалена вакансия: \"{vacancyName}\"";

            var mailModel = CreateMailopostModelConfirmEmail(mailTo, text, subject, text);
            await SendEmailNotificationAsync(mailModel);
        }
    }

    /// <summary>
    /// Метод отправляет уведомление на почту владельца проекта о удалении проекта и всего связанного с ним.
    /// </summary>
    /// <param name="mailTo">Почта владельца проекта.</param>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="vacanciesNames">Список названий вакансий, которые были также удалены.</param>
    public async Task SendNotificationDeleteProjectAsync(string mailTo, string projectName, 
        List<string> vacanciesNames)
    {
        var isEnabledEmailNotifications = await _globalConfigRepository
            .GetValueByKeyAsync<bool>(GlobalConfigKeys.EmailNotifications.EMAIL_NOTIFICATIONS_DISABLE_MODE_ENABLED);

        if (isEnabledEmailNotifications)
        {
            var deleteVacanciesBuilder = new StringBuilder();
            
            if (vacanciesNames.Any())
            {
                deleteVacanciesBuilder.AppendLine("<br/>");
                deleteVacanciesBuilder.AppendLine("Удалена связь следующих вакансий с проектом: ");
                deleteVacanciesBuilder.AppendLine("<br/>");
                
                foreach (var vac in vacanciesNames)
                {
                    deleteVacanciesBuilder.AppendLine("- " + vac);
                    deleteVacanciesBuilder.AppendLine("<br/>");
                }
            }
            
            // TODO: Заменить на получение ссылки из БД.
            var text = $"Вы удалили проект: \"{projectName}\"." +
                       deleteVacanciesBuilder +
                       "<br/>" +
                       "<br/>" +
                       "<br/>" +
                       "<br/>-----<br/>" +
                       "С уважением, команда Leoka Estetica";
            var subject = $"Удален проект: \"{projectName}\"";

            var mailModel = CreateMailopostModelConfirmEmail(mailTo, text, subject, text);
            await SendEmailNotificationAsync(mailModel);
        }
    }

    /// <summary>
    /// Метод отправляет уведомление на почту владельца проекта об отклике на его проект.
    /// </summary>
    /// <param name="mailTo">Почта владельца проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="otherUser">Логин или почта пользователя, который оставил отклик.</param>
    public async Task SendNotificationWriteResponseProjectAsync(string mailTo, long projectId, string projectName,
        string vacancyName, string otherUser)
    {
        var isEnabledEmailNotifications = await _globalConfigRepository
            .GetValueByKeyAsync<bool>(GlobalConfigKeys.EmailNotifications.EMAIL_NOTIFICATIONS_DISABLE_MODE_ENABLED);

        if (isEnabledEmailNotifications)
        {
            var withVacancy = !string.IsNullOrEmpty(vacancyName)
                ? $"Отклик на вакансию: \"{vacancyName}\""
                : null;
            
            // TODO: Заменить на получение ссылки из БД.
            var text = $"Пользователь {otherUser} оставил отклик на ваш проект: \"{projectName}\"." +
                       "<br/>" +
                        withVacancy +
                       "<br/>" +
                       $"<a href='https://leoka-estetica-dev.ru/projects/project?projectId={projectId}&mode=view'>" +
                       "Перейти к проекту" +
                       "</a>" +
                       "<br/>" +
                       "<br/>" +
                       "<br/>" +
                       "<br/>-----<br/>" +
                       "С уважением, команда Leoka Estetica";
            var subject = $"Новый отклик на ваш проект: \"{projectName}\"";

            var mailModel = CreateMailopostModelConfirmEmail(mailTo, text, subject, text);
            await SendEmailNotificationAsync(mailModel);
        }
    }

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод создает входную модель с параметрами для отправки письма подтверждения почты.
    /// </summary>
    /// <param name="mailTo">Кому отправить.</param>
    /// <returns>Модель с данными.</returns>
    private MailopostInput CreateMailopostModelConfirmEmail(string mailTo, string html, string subject, string text)
    {
        var model = new MailopostInput
        {
            FromEmail = _configuration["MailingsSettings:Mailopost:FromEmail"],
            FromName = _configuration["MailingsSettings:Mailopost:FromName"],
            To = mailTo,
            Html = html,
            Subject = subject,
            Text = text,
            Payment = "credit"
        };

        return model;
    }

    /// <summary>
    /// TODO: Вынести куда то, так как дубли в модерации есть у этого метода.
    /// Метод отправляет уведомление на почту пользователя.
    /// </summary>
    /// <param name="serializeObject">Сериализованный объект для отправки.</param>
    private async Task SendEmailNotificationAsync(object serializeObject)
    {
        var request = WebRequest.Create(_configuration["MailingsSettings:Mailopost:ApiUrl"]
                                        + ApiMailopostConsts.SEND_MESSAGE);
        request.Method = "POST";
        request.Headers.Add("Authorization", "Bearer " + _configuration["MailingsSettings:Mailopost:ApiKey"]);
        var json = JsonConvert.SerializeObject(serializeObject);
        var byteArray = Encoding.UTF8.GetBytes(json);
        request.ContentType = "application/json";
        request.ContentLength = byteArray.Length;
        
        // Записываем данные в поток.
        await using var dataStream = await request.GetRequestStreamAsync();
        await dataStream.WriteAsync(byteArray);
        using var response = await request.GetResponseAsync();
        await using var stream = response.GetResponseStream();
        using var reader = new StreamReader(stream);   
    }

    #endregion
}