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
    
    /// <summary>
    /// Метод получает список таймлайнов.
    /// </summary>
    /// <returns>Список таймлайнов.</returns>
    Task<Dictionary<string, List<TimelineEntity>>> GetTimelinesAsync();

    /// <summary>
    /// Метод получает преимущества платформы.
    /// </summary>
    /// <returns>Преимущества платформы.</returns>
    Task<IEnumerable<PlatformConditionEntity>> GetPlatformConditionsAsync();
}