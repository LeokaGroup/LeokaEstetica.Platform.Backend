using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace LeokaEstetica.Platform.Base.Extensions;

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
}