using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Input.User;

/// <summary>
/// Класс входной модели авторизации через VK.
/// </summary>
public class UserSignInVkInput
{
    /// <summary>
    /// Id пользователя.
    /// </summary>
    [JsonProperty("uid")]
    public long UserId { get; set; }
    
    /// <summary>
    /// Имя.
    /// </summary>
    [JsonProperty("first_name")]
    public string FirstName { get; set; }

    /// <summary>
    /// Фамилия.
    /// </summary>
    [JsonProperty("last_name")]
    public string LastName { get; set; }
}