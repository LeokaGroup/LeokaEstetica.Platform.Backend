using LeokaEstetica.Platform.Models.Dto.Output.Header;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Services.Abstractions.Header;

/// <summary>
/// Абстракция сервиса работы с хидером.
/// </summary>
public interface IHeaderService
{
    /// <summary>
    /// Метод получает список элементов для меню хидера в зависимости от его типа.
    /// </summary>
    /// <param name="headerType">Тип хидера. Например, для лендоса.</param>
    /// <returns>Список элементов для меню хидера.</returns>
    Task<IEnumerable<HeaderOutput>> HeaderItemsAsync(HeaderTypeEnum headerType);
}