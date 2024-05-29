using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace LeokaEstetica.Platform.Base.Extensions;

/// <summary>
/// Класс расширений для Http-клиента.
/// </summary>
public static class HttpClientExtensions
{
    public static HttpClient SetYandexKassaRequestAuthorizationHeader(this HttpClient httpClient, IConfiguration configuration)
    {
        // Устанавливаем заголовки запросу.
        var encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
            .GetBytes(configuration["Commerce:UKassa:ShopId"] + ":" + configuration["Commerce:UKassa:ApiToken"]));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encoded);
        httpClient.DefaultRequestHeaders.Add("Idempotence-Key", Guid.NewGuid().ToString());

        return httpClient;
    }
    
    /// <summary>
    /// Метод устанавливает заголовки авторизации Http-запросу.
    /// </summary>
    /// <param name="httpClient">Http-клиент.</param>
    /// <param name="accessToken">Токен доступа.</param>
    /// <returns>Http-клиент с установленным заголовком авторизации.</returns>
    public static HttpClient SetHttpClientRequestAuthorizationHeader(this HttpClient httpClient, string accessToken)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return httpClient;
    }
}