using System.Net;
using System.Text;
using LeokaEstetica.Platform.Base.Abstractions.Messaging.Mail;
using LeokaEstetica.Platform.Base.Models.Input.Messaging.Mail;
using LeokaEstetica.Platform.Core.Constants;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Base.Abstractions.Services.Messaging.Mail;

public class MailingsService : IMailingsService
{
    private readonly IConfiguration _configuration;

    public MailingsService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

   /// <summary>
    /// Метод отправляет уведомление на почту о принятии инвайта в проект.
    /// </summary>
    /// <param name="mailTo">Почта пользователя, которому отправлять уведомление.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="otherUser">Логин или почта пользователя, который оставил отклик.</param>
   /// <param name="isEmailNotificationsDisableModeEnabled">Признак уведомлений на почту.</param>
    public async Task SendNotificationApproveInviteProjectAsync(string mailTo, long projectId, string projectName,
        string vacancyName, string otherUser, bool isEmailNotificationsDisableModeEnabled)
    {
        if (isEmailNotificationsDisableModeEnabled)
        {
            var vacancy = !string.IsNullOrEmpty(vacancyName) ? $"Вакансия: {vacancyName}" + "<br/>" : null;
            
            // TODO: Заменить на получение ссылки из БД.
            var text = $"Пользователь {otherUser} принял приглашение в проект: \"{projectName}\"." +
                       "<br/>" +
                       vacancy +
                       $"<a href='https://leoka-estetica-dev.ru/projects/project?projectId={projectId}&mode=view'>" +
                       "Перейти к проекту" +
                       "</a>" +
                       "<br/>" +
                       "<br/>" +
                       "<br/>" +
                       "<br/>-----<br/>" +
                       "С уважением, команда Leoka Estetica";
            var subject = $"Приглашение в команду проекта: \"{projectName}\"";

            var mailModel = CreateMailopostModelConfirmEmail(mailTo, text, subject, text);
            await SendEmailNotificationAsync(mailModel);
        }
    }

    /// <summary>
    /// Метод отправляет уведомление на почту о отклонении инвайта в проект.
    /// </summary>
    /// <param name="mailTo">Почта пользователя, которому отправлять уведомление.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="otherUser">Логин или почта пользователя, который оставил отклик.</param>
    /// <param name="isEmailNotificationsDisableModeEnabled">Признак уведомлений на почту.</param>
    public async Task SendNotificationRejectInviteProjectAsync(string mailTo, long projectId, string projectName,
        string vacancyName, string otherUser, bool isEmailNotificationsDisableModeEnabled)
    {
        if (isEmailNotificationsDisableModeEnabled)
        {
            var vacancy = !string.IsNullOrEmpty(vacancyName) ? $"Вакансия: {vacancyName}" + "<br/>" : null;
            
            // TODO: Заменить на получение ссылки из БД.
            var text = $"Пользователь {otherUser} отклонил приглашение в проект: \"{projectName}\"." +
                       "<br/>" +
                       vacancy +
                       $"<a href='https://leoka-estetica-dev.ru/projects/project?projectId={projectId}&mode=view'>" +
                       "Перейти к проекту" +
                       "</a>" +
                       "<br/>" +
                       "<br/>" +
                       "<br/>" +
                       "<br/>-----<br/>" +
                       "С уважением, команда Leoka Estetica";
            var subject = $"Приглашение в команду проекта: \"{projectName}\"";

            var mailModel = CreateMailopostModelConfirmEmail(mailTo, text, subject, text);
            await SendEmailNotificationAsync(mailModel);
        }
    }
    
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
}