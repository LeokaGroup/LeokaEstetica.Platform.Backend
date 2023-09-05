namespace LeokaEstetica.Platform.Models.Dto.Output.User;

/// <summary>
/// Класс выходной модели конфигурации провайдеров аутентификации.
/// </summary>
public class AuthProviderConfigOutput
{
    /// <summary>
    /// Ссылка аутентификации через ВК.
    /// </summary>
    public string VkReference { get; set; }
    
    /// <summary>
    /// Ссылка аутентификации через ВК после успешной аутентификации.
    /// </summary>
    public string VkRedirectReference { get; set; }
    
    /// <summary>
    /// Ссылка аутентификации через Google.
    /// </summary>
    public string GoogleReference { get; set; }
    
    /// <summary>
    /// Ссылка аутентификации через Google после успешной аутентификации.
    /// </summary>
    public string GoogleRedirectReference { get; set; }
}