using LeokaEstetica.Platform.Models.Entities.Configs;

namespace LeokaEstetica.Platform.Base.Abstractions.Repositories;

/// <summary>
/// Абстракция репозитория для исключения параметров валидации, которые не нужно выдавать фронту.
/// </summary>
public interface IValidationExcludeErrorsRepository
{
    /// <summary>
    /// Метод получает список полей, которые нужно исключить.
    /// </summary>
    /// <returns>Список полей для исключения.</returns>
    Task<IEnumerable<ValidationColumnExcludeEntity>> ValidationColumnsExcludeAsync();
}