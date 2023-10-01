using LeokaEstetica.Platform.Models.Entities.Communication;

namespace LeokaEstetica.Platform.Database.Abstractions.Press;

/// <summary>
/// Абстракция репозитория прессы.
/// </summary>
public interface IPressRepository
{
    /// <summary>
    /// Метод получает список контактов.
    /// </summary>
    /// <returns>Список контактов.</returns>
    Task<IEnumerable<ContactEntity>> GetContactsAsync();

    /// <summary>
    /// Метод получает данные публичной оферты.
    /// </summary>
    /// <returns>Данные публичной оферты.</returns>
    Task<IEnumerable<PublicOfferEntity>> GetPublicOfferAsync();
}