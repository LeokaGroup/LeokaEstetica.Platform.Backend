using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Enums;
using Enum = System.Enum;

namespace LeokaEstetica.Platform.Controllers.Validators.Access;

/// <summary>
/// Класс валидатора проверки доступа к модулю и к компонентам модуля.
/// </summary>
public class CheckAccessProjectManagementModuleOrComponentValidator : AbstractValidator<(long ProjectId,
    AccessModuleTypeEnum AccessModuleType, AccessModuleComponentTypeEnum AccessModuleComponentType)>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public CheckAccessProjectManagementModuleOrComponentValidator()
    {
        RuleFor(p => p.ProjectId)
            .NotNull()
            .WithMessage(AccessConst.NOT_VALID_PROJECT_ID)
            .Must(p => p > 0)
            .WithMessage(AccessConst.NOT_VALID_PROJECT_ID);

        RuleFor(x => x.AccessModuleType)
            .Must(i => Enum.IsDefined(typeof(AccessModuleTypeEnum), i))
            .WithMessage(AccessConst.NOT_VALID_MODULE_TYPE);
            
        RuleFor(x => x.AccessModuleComponentType)
            .Must(i => Enum.IsDefined(typeof(AccessModuleComponentTypeEnum), i))
            .WithMessage(AccessConst.NOT_VALID_MODULE_COMPONENT_TYPE);
    }
}