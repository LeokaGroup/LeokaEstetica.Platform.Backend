using LeokaEstetica.Platform.Abstractions.Header;
using LeokaEstetica.Platform.Models.Dto.Output.Header;

namespace LeokaEstetica.Platform.Services.Header;

public sealed class HeaderService : IHeaderService
{
    public HeaderService()
    {
    }

    /// <summary>
    /// Метод получает список элементов для хидера в зависимости от его типа.
    /// </summary>
    /// <param name="headerType">Тип хидера.</param>
    /// <returns>Список элементов хидера.</returns>
    public Task<IEnumerable<HeaderOutput>> HeaderItemsAsymc(int headerType)
    {
        throw new NotImplementedException();
    }
}