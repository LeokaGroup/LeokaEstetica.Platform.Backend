using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

namespace LeokaEstetica.Platform.Processing.Extensions;

/// <summary>
/// Класс расширений для http-клиента.
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    /// Метод устанавливает запросу заголовки авторизации.
    /// </summary>
    /// <param name="httpClient"></param>
    /// <param name="configuration"></param>
    public static void SetPayMasterRequestAuthorizationHeader(this HttpClient httpClient, IConfiguration configuration)
    {
        // Устанавливаем заголовки запросу.
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
            configuration["Commerce:PayMaster:ApiToken"]);
    }
}