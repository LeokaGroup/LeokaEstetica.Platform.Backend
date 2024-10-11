using LeokaEstetica.Platform.Models.Dto.Communications.Output;

namespace LeokaEstetica.Platform.Database.Abstractions.Communications;

/// <summary>
/// Абстракция репозитория абстрактных областей чата.
/// </summary>
public interface IAbstractScopeRepository
{
    /// <summary>
    /// Метод получает список абстрактных областей чата.
    /// Учитывается текущий пользователь.
    /// Текущий метод можно расширять новыми абстрактными областями.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список абстрактных областей чата.</returns>
    Task<IEnumerable<AbstractScopeOutput>> GetAbstractScopesAsync(long userId);
}