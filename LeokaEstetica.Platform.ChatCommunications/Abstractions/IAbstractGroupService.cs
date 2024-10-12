using LeokaEstetica.Platform.Models.Dto.Communications.Output;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Communications.Abstractions;

/// <summary>
/// Абстракция сервиса групп абстрактной области чата.
/// </summary>
public interface IAbstractGroupService
{
    /// <summary>
    /// Метод получает объекты группы абстрактной области.
    /// </summary>
    /// <param name="abstractScopeId">Id абстрактной области.</param>
    /// <param name="abstractScopeType">Тип абстрактной области.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Объекты группы абстрактной области.</returns>
    Task<AbstractGroupResult> GetAbstractGroupObjectsAsync(long abstractScopeId,
        AbstractScopeTypeEnum abstractScopeType, string account);
}