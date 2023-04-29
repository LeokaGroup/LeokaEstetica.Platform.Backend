using System.Net;
using System.Text;
using LeokaEstetica.Platform.CallCenter.Abstractions.Messaging.Mail;
using LeokaEstetica.Platform.CallCenter.Models.Dto.Input.Messaging.Mail;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.CallCenter.Services.Messaging.Mail;

/// <summary>
/// Класс реализует методы сервиса уведомлений почты модерации.
/// </summary>
public class ModerationMailingsService : IModerationMailingsService
{
    private readonly IGlobalConfigRepository _globalConfigRepository;
    private readonly IConfiguration _configuration;
    
    public ModerationMailingsService(IGlobalConfigRepository globalConfigRepository, 
        IConfiguration configuration)
    {
        _globalConfigRepository = globalConfigRepository;
        _configuration = configuration;
    }

    /// <summary>
    /// Метод отправляет уведомление на почту владельца проекта о одобрении проекта модератором.
    /// </summary>
    /// <param name="mailTo">Почта владельца проекта.</param>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    public async Task SendNotificationApproveProjectAsync(string mailTo, string projectName, long projectId)
    {
        var isEnabledEmailNotifications = await _globalConfigRepository
            .GetValueByKeyAsync<bool>(GlobalConfigKeys.EmailNotifications.EMAIL_NOTIFICATIONS_DISABLE_MODE_ENABLED);

        if (isEnabledEmailNotifications)
        {
            // TODO: Заменить на получение ссылки из БД.
            var text = $"Модератор одобрил ваш проект: \"{projectName}\"." +
                       "<br/>" +
                       $"<a href='https://leoka-estetica-dev.ru/projects/project?projectId={projectId}&mode=view'>" +
                       "Перейти к проекту" +
                       "</a>" +
                       "<br/>" +
                       "<br/>" +
                       "<br/>-----<br/>" +
                       "С уважением, команда Leoka Estetica";
            var subject = $"Ваш проект: \"{projectName}\" одобрен модератором";

            var mailModel = CreateMailopostModelConfirmEmail(mailTo, text, subject, text);
            await SendEmailNotificationAsync(mailModel);
        }
    }

    /// <summary>
    /// Метод отправляет уведомление на почту владельца проекта о одобрении проекта модератором.
    /// </summary>
    /// <param name="mailTo">Почта владельца проекта.</param>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    public async Task SendNotificationRejectProjectAsync(string mailTo, string projectName, long projectId)
    {
        var isEnabledEmailNotifications = await _globalConfigRepository
            .GetValueByKeyAsync<bool>(GlobalConfigKeys.EmailNotifications.EMAIL_NOTIFICATIONS_DISABLE_MODE_ENABLED);

        if (isEnabledEmailNotifications)
        {
            // TODO: Заменить на получение ссылки из БД.
            var text = $"Модератор отклонил ваш проект: \"{projectName}\"." +
                       "<br/>" +
                       $"<a href='https://leoka-estetica-dev.ru/projects/project?projectId={projectId}&mode=view'>" +
                       "Перейти к проекту" +
                       "</a>" +
                       "<br/>" +
                       "<br/>" +
                       "<br/>-----<br/>" +
                       "С уважением, команда Leoka Estetica";
            var subject = $"Ваш проект: \"{projectName}\" отклонен модератором";

            var mailModel = CreateMailopostModelConfirmEmail(mailTo, text, subject, text);
            await SendEmailNotificationAsync(mailModel);
        }
    }

    /// <summary>
    /// Метод отправляет уведомление на почту владельца вакансии о одобрении вакансии модератором.
    /// </summary>
    /// <param name="mailTo">Почта владельца вакансии.</param>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    public async Task SendNotificationApproveVacancyAsync(string mailTo, string vacancyName, long vacancyId)
    {
        var isEnabledEmailNotifications = await _globalConfigRepository
            .GetValueByKeyAsync<bool>(GlobalConfigKeys.EmailNotifications.EMAIL_NOTIFICATIONS_DISABLE_MODE_ENABLED);

        if (isEnabledEmailNotifications)
        {
            // TODO: Заменить на получение ссылки из БД.
            var text = $"Модератор одобрил вашу вакансию: \"{vacancyName}\"." +
                       "<br/>" +
                       $"<a href='https://leoka-estetica-dev.ru/vacancies/vacancy?vacancyId={vacancyId}&mode=view'>" +
                       "Перейти к вакансии" +
                       "</a>" +
                       "<br/>" +
                       "<br/>" +
                       "<br/>-----<br/>" +
                       "С уважением, команда Leoka Estetica";
            var subject = $"Ваша вакансия: \"{vacancyName}\" одобрена модератором";

            var mailModel = CreateMailopostModelConfirmEmail(mailTo, text, subject, text);
            await SendEmailNotificationAsync(mailModel);
        }
    }

    /// <summary>
    /// Метод отправляет уведомление на почту владельца вакансии о отклонении вакансии модератором.
    /// </summary>
    /// <param name="mailTo">Почта владельца вакансии.</param>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    public async Task SendNotificationRejectVacancyAsync(string mailTo, string vacancyName, long vacancyId)
    {
        var isEnabledEmailNotifications = await _globalConfigRepository
            .GetValueByKeyAsync<bool>(GlobalConfigKeys.EmailNotifications.EMAIL_NOTIFICATIONS_DISABLE_MODE_ENABLED);

        if (isEnabledEmailNotifications)
        {
            // TODO: Заменить на получение ссылки из БД.
            var text = $"Модератор отклонил вашу вакансию: \"{vacancyName}\"." +
                       "<br/>" +
                       $"<a href='https://leoka-estetica-dev.ru/vacancies/vacancy?vacancyId={vacancyId}&mode=view'>" +
                       "Перейти к вакансии" +
                       "</a>" +
                       "<br/>" +
                       "<br/>" +
                       "<br/>-----<br/>" +
                       "С уважением, команда Leoka Estetica";
            var subject = $"Ваша вакансия: \"{vacancyName}\" отклонена модератором";

            var mailModel = CreateMailopostModelConfirmEmail(mailTo, text, subject, text);
            await SendEmailNotificationAsync(mailModel);
        }
    }
    
    /// <summary>
    /// Метод отправляет уведомление на почту пользователя, которого внесли в чёрный список.
    /// </summary>
    /// <param name="mailTo"></param>
    public async Task SendNotificationBlockUserAccountAsync(string mailTo)
    {
        var isEnabledEmailNotifications = await _globalConfigRepository
            .GetValueByKeyAsync<bool>(GlobalConfigKeys.EmailNotifications.EMAIL_NOTIFICATIONS_DISABLE_MODE_ENABLED);

        if (isEnabledEmailNotifications)
        {
            var subject = "Блокировка аккаунта Leoka Estetica.";
            var html = "Ваш аккаунт на Leoka Estetica был заблокирован администратором. " +
                       "О причинах вы можете узнать у тех.поддержки.";

            var mailModel = CreateMailopostModelConfirmEmail(mailTo, html, subject, html);
            await SendEmailNotificationAsync(mailModel);
        }
    }

    #region Приватные методы.

        /// <summary>
        /// TODO: Есть дубль в проекте Messaging, потом отрефачить.
        /// TODO: Из-за референсов нельзя переиспользовать, цикличные ссылки.
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
        /// TODO: Есть дубль в проекте Messaging, потом отрефачить.
        /// TODO: Из-за референсов нельзя переиспользовать, цикличные ссылки.
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