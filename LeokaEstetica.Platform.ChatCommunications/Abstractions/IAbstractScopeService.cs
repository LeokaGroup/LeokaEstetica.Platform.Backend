using LeokaEstetica.Platform.Models.Dto.Communications.Output;

namespace LeokaEstetica.Platform.Communications.Abstractions;

/// <summary>
/// Абстракция сервиса абстрактных областей чата.
/// </summary>
public interface IAbstractScopeService
{
    /// <summary>
    /// Метод получает список абстрактных областей чата.
    /// Учитывается текущий пользователь.
    /// Текущий метод можно расширять новыми абстрактными областями.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список абстрактных областей чата.</returns>
    Task<IEnumerable<AbstractScopeOutput>> GetAbstractScopesAsync(string account);
}