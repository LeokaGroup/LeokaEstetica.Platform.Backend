using FluentValidation.Results;

namespace LeokaEstetica.Platform.Base.Abstractions.Services.Validation;

/// <summary>
/// Абстракция сервиса для исключения параметров валидации, которые не нужно выдавать фронту.
/// </summary>
public interface IValidationExcludeErrorsService
{
    /// <summary>
    /// Метод исключает из списка ошибок те, которые есть в управляющей таблице исключения параметров валидации.
    /// </summary>
    /// <param name="errors">Список ошибок.</param>
    Task<List<ValidationFailure>> ExcludeAsync(List<ValidationFailure> errors);
}