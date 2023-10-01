using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Press;
using LeokaEstetica.Platform.Models.Entities.Communication;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Press;

/// <summary>
/// Класс реализует методы репозитория прессы.
/// </summary>
internal sealed class PressRepository : IPressRepository
{
    private readonly PgContext _pgContext;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    public PressRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод получает список контактов.
    /// </summary>
    /// <returns>Список контактов.</returns>
    public async Task<IEnumerable<ContactEntity>> GetContactsAsync()
    {
        var result = await _pgContext.Contacts.OrderBy(o => o.ContactId).ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает данные публичной оферты.
    /// </summary>
    /// <returns>Данные публичной оферты.</returns>
    public async Task<IEnumerable<PublicOfferEntity>> GetPublicOfferAsync()
    {
        var result = await _pgContext.PublicOffer.OrderBy(o => o.OfferId).ToListAsync();

        return result;
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}