using FluentValidation.Results;
using LeokaEstetica.Platform.Models.Dto.Common;

namespace LeokaEstetica.Platform.Models.Dto.Output.Profile;

/// <summary>
/// Класс выходной модели сохранения списка навыков пользователя в анкете.
/// </summary>
public class SaveUserSkillOutput : IFrontError
{
    /// <summary>
    /// Список навыков.
    /// </summary>
    public List<SkillOutput> ProfileSkills { get; set; }

    /// <summary>
    /// Успешно ли сохранение.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<ValidationFailure> Errors { get; set; }
}