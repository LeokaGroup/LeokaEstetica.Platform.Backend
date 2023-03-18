using LeokaEstetica.Platform.Models.Dto.Output.Landing;

namespace LeokaEstetica.Platform.Services.Abstractions.Landing;

/// <summary>
/// Абстракция сервиса лендингов.
/// </summary>
public interface ILandingService
{
    /// <summary>
    /// Метод получает данные для блока фона для главного лендоса.
    /// </summary>
    /// <returns>Данные блока.returns>
    Task<LandingStartFonOutput> LandingStartFonAsync();

    /// <summary>
    /// Метод получает данные для фона предложений платформы.
    /// </summary>
    /// <returns>Данные для фона.</returns>
    Task<PlatformOfferOutput> GetPlatformItemsAsync();
}