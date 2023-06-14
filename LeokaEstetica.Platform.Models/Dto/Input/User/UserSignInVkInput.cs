using System.Text.Json.Serialization;

namespace LeokaEstetica.Platform.Models.Dto.Input.User;

/// <summary>
/// Класс входной модели авторизации через VK.
/// </summary>
public class UserSignInVkInput
{
    /// <summary>
    /// Id пользователя в системе ВК.
    /// </summary>
    [JsonPropertyName("uid")]
    public long VkUserId { get; set; }
    
    /// <summary>
    /// Имя.
    /// </summary>
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }

    /// <summary>
    /// Фамилия.
    /// </summary>
    [JsonPropertyName("last_name")]
    public string LastName { get; set; }
}