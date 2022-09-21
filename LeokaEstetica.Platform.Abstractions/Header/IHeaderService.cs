using LeokaEstetica.Platform.Models.Dto.Output.Header;

namespace LeokaEstetica.Platform.Abstractions.Header;

/// <summary>
/// Абстракция работы с хидерами приложения.
/// </summary>
public interface IHeaderService
{
    /// <summary>
    /// Метод получает список элементов для хидера в зависимости от его типа.
    /// </summary>
    /// <param name="headerType">Тип хидера.</param>
    /// <returns>Список элементов хидера.</returns>
    Task<IEnumerable<HeaderOutput>> HeaderItemsAsymc(int headerType);
}