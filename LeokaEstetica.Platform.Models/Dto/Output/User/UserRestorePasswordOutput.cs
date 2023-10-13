using FluentValidation.Results;
using LeokaEstetica.Platform.Models.Dto.Common;

namespace LeokaEstetica.Platform.Models.Dto.Output.User;

/// <summary>
/// Класс выходной модели восстановления пароля пользователя.
/// </summary>
public class UserRestorePasswordOutput : IFrontError
{
    /// <summary>
    /// Флаг успеха авторизации.
    /// </summary>
    public bool IsSuccess { get; set; }
    
    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<ValidationFailure> Errors { get; set; }
}