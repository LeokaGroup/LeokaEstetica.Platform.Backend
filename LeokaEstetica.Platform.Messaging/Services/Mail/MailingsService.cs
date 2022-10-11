using System.Net;
using System.Text;
using LeokaEstetica.Platform.Messaging.Abstractions.Mail;
using LeokaEstetica.Platform.Messaging.Consts;
using LeokaEstetica.Platform.Messaging.Models.Mail.Input.Mailopost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Messaging.Services.Mail;

/// <summary>
/// Класс реализует методы сервиса работы с сообщениями.
/// </summary>
public sealed class MailingsService : IMailingsService
{
    private readonly IConfiguration _configuration;

    public MailingsService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Метод отправит подтверждение на почту.
    /// </summary>
    /// <param name="mailTo">Email кому отправить.</param>
    /// <param name="confirmEmailCode">Код подтверждения почты.</param>
    public async Task SendConfirmEmailAsync(string mailTo, Guid confirmEmailCode)
    {
        var mailModel = CreateMailopostModelConfirmEmail(mailTo, confirmEmailCode);
        var request = WebRequest.Create(_configuration["MailingsSettings:Mailopost:ApiUrl"] + ApiMailopostConsts.SEND_MESSAGE);
        request.Method = "POST";
        request.Headers.Add("Authorization", "Bearer " + _configuration["MailingsSettings:Mailopost:ApiKey"]);
        var json = JsonConvert.SerializeObject(mailModel);
        var byteArray = Encoding.UTF8.GetBytes(json);
        request.ContentType = "application/json";
        request.ContentLength = byteArray.Length;
        
        // Записываем данные в поток.
        await using var dataStream = await request.GetRequestStreamAsync();
        await dataStream.WriteAsync(byteArray);
        using var response = await request.GetResponseAsync();
        await using var stream = response.GetResponseStream();
        using var reader = new StreamReader(stream);
        
        // Получаем результат.
        // var result = await reader.ReadToEndAsync();
    }

    /// <summary>
    /// Метод создает входную модель с параметрами для отправки письма подтверждения почты.
    /// </summary>
    /// <param name="mailTo">Кому отправить.</param>
    /// <param name="confirmEmailCode">Код подтверждения.</param>
    /// <returns>Модель с данными.</returns>
    private MailopostInput CreateMailopostModelConfirmEmail(string mailTo, Guid confirmEmailCode)
    {
        var model = new MailopostInput
        {
            FromEmail = _configuration["MailingsSettings:Mailopost:FromEmail"],
            FromName = _configuration["MailingsSettings:Mailopost:FromName"],
            To = mailTo,
            Html = "Рады вас видеть на Leoka Estetica!" +
                   "<br/><br/>" +
                   $"Для завершения регистрации перейдите по ссылке <a href='https://leoka-estetica-dev.ru/user/confirm-email?code={confirmEmailCode}'>Подтвердить почту</a>" +
                   "<br/>-----<br/>" +
                   "С уважением, команда Leoka Estetica",
            Subject = "Активация аккаунта на leoka-estetica.ru",
            Text = "Рады вас видеть на Leoka Estetica!" +
                   "<br/><br/>" +
                   $"Для завершения регистрации перейдите по ссылке <a href='https://leoka-estetica-dev.ru/user/confirm-email?code={confirmEmailCode}'>Подтвердить почту</a>" +
                   "<br/>-----<br/>" +
                   "<br/>" +
                   "<br/>" +
                   "С уважением, команда Leoka Estetica",
            Payment = "credit"
        };

        return model;
    }
}