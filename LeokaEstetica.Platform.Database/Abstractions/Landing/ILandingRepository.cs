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
}