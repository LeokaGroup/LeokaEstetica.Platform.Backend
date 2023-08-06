using System.Net;
using System.Text;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using LeokaEstetica.Platform.Controllers.Factories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Controllers.Middlewares;

/// <summary>
/// Обработчик ошибок API.
/// </summary>
public class ApiErrorHandler : IExceptionHandler
{
    private static IConfiguration _configuration;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="configuration">Конфигурация.</param>
    public ApiErrorHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Метод обработки исключения.
    /// </summary>
    /// <param name="context">Текущий контекст.</param>
    /// <param name="cancellationToken">Токен отмены задачи.</param>
    public async Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
    {
        context.Result = new ExceptionInfo(context);

        await Task.CompletedTask;
    }
    
    /// <summary>
    /// exception приведененый к http ошибке.
    /// Является ответом обработчика.
    /// </summary>
    private class ExceptionInfo : IHttpActionResult
    {
        /// <summary>
        /// Запрос.
        /// </summary>
        private HttpRequestMessage Request { get; set; }

        /// <summary>
        /// Сообщение об ошибке.
        /// </summary>
        private ErrorMessage ExceptionMessage { get; set; }

        /// <summary>
        /// Код статуса ошибки.
        /// </summary>
        private HttpStatusCode HttpStatusCode { get; set; }

        public ExceptionInfo(ExceptionHandlerContext context)
        {
            Request = context.ExceptionContext.Request;
            ExceptionMessage = CreateErrorMessageFactory.Create(context, _configuration);
            HttpStatusCode = HttpStatusCode.InternalServerError; // Для API отдаем всегда 500.
        }

        /// <summary>
        /// Ответ обработчика.
        /// </summary>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode);
            response.Content = new StringContent(JsonConvert.SerializeObject(ExceptionMessage), Encoding.UTF8,
                "application/json");
            response.RequestMessage = Request;

            return Task.FromResult(response);
        }
    }
    
    /// <summary>
    /// Класс описывает ошибку исключения.
    /// </summary>
    public class ErrorMessage
    {
        /// <summary>
        /// Текст ошибки.
        /// </summary>
        public string ExceptionMessage;

        /// <summary>
        /// Тип ошибки.
        /// </summary>
        public string ExceptionType;

        /// <summary>
        /// Идентификатор запроса.
        /// </summary>
        public Guid TraceId;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="message">Сообщение ошибки.</param>
        public ErrorMessage(string message)
        {
            ExceptionMessage = message;
        }
    }
}