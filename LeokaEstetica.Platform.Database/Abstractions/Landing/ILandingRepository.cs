using LeokaEstetica.Platform.Models.Entities.Landing;

namespace LeokaEstetica.Platform.Database.Abstractions.Landing;

/// <summary>
/// Абстракция репозитория лендингов для работы с БД.
/// </summary>
public interface ILandingRepository
{
    /// <summary>
    /// Метод получает данные для блока фона для главного лендоса.
    /// </summary>
    /// <returns>Данные блока.returns>
    Task<FonEntity> LandingStartFonAsync();
    
    /// <summary>
    /// Метод получает данные для фона предложений платформы.
    /// </summary>
    /// <returns>Данные для фона.</returns>
    Task<PlatformOfferEntity> GetPlatformDataAsync();
    
    /// <summary>
    /// Метод получает список элементов для фона предложений платформы.
    /// </summary>
    /// <returns>Данные для фона.</returns>
    Task<IEnumerable<PlatformOfferItemsEntity>> GetPlatformItemsAsync();
}