using FluentValidation.Results;
using LeokaEstetica.Platform.Models.Dto.Common;

namespace LeokaEstetica.Platform.Models.Dto.Output.Project;

/// <summary>
/// Класс выходной модели для прикрепления вакансии к проекту.
/// </summary>
public class AttachProjectVacancyOutput : IFrontError
{
    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<ValidationFailure> Errors { get; set; }
}