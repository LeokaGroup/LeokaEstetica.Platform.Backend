using LeokaEstetica.Platform.Models.Entities.Common;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Database.Abstractions.Header;

/// <summary>
/// Абстракция репозитория хидера для работы с БД.
/// </summary>
public interface IHeaderRepository
{
    /// <summary>
    /// Метод получает список элементов для меню хидера в зависимости от его типа.
    /// </summary>
    /// <param name="headerType">Тип хидера. Например, для лендоса.</param>
    /// <returns>Список элементов для меню хидера.</returns>
    Task<IEnumerable<HeaderEntity>> HeaderItemsAsync(HeaderTypeEnum headerType);
}