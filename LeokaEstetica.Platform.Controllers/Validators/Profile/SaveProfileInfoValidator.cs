using System.Text.RegularExpressions;
using FluentValidation;
using LeokaEstetica.Platform.Models.Dto.Input.Profile;
using LeokaEstetica.Platform.Services.Consts;
using NPOI.SS.Formula.Functions;

namespace LeokaEstetica.Platform.Controllers.Validators.Profile;

/// <summary>
/// Класс валидатора сохранения информации профиля.
/// </summary>
public class SaveProfileInfoValidator : AbstractValidator<ProfileInfoInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public SaveProfileInfoValidator()
    {//фамилия - имя - mail - номер - телефон - о себе - должность - стаж - навыки - цели
        RuleFor(p => p.FirstName)
            .NotNull()
            .WithMessage(ValidationConsts.EMPTY_FIRST_NAME_ERROR)
            .NotEmpty()
            .WithMessage(ValidationConsts.EMPTY_FIRST_NAME_ERROR);
        
        RuleFor(p => p.LastName)
            .NotNull()
            .WithMessage(ValidationConsts.EMPTY_LAST_NAME_ERROR)
            .NotEmpty()
            .WithMessage(ValidationConsts.EMPTY_LAST_NAME_ERROR);

        RuleFor(p => p.Email)
            .NotNull()
            .WithMessage(ValidationConsts.EMPTY_EMAIL_ERROR)
            .NotEmpty()
            .WithMessage(ValidationConsts.EMPTY_EMAIL_ERROR)
            .Matches("[.\\-_a-z0-9]+@([a-z0-9][\\-a-z0-9]+\\.)+[a-z]{2,6}", RegexOptions.IgnoreCase);

        RuleFor(p => p.PhoneNumber)
            .NotNull()
            .WithMessage(ValidationConsts.EMPTY_PHONE_NUMBER_ERROR)
            .NotEmpty()
            .WithMessage(ValidationConsts.EMPTY_PHONE_NUMBER_ERROR)
            .Matches(@"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$");

        RuleFor(p => p.Aboutme)
            .NotNull()
            .WithMessage(ValidationConsts.EMPTY_ABOUTME_ERROR)
            .NotEmpty()
            .WithMessage(ValidationConsts.EMPTY_ABOUTME_ERROR);

        RuleFor(p => p.Job)//должность
           .NotNull()
           .WithMessage(ValidationConsts.EMPTY_JOB_ERROR)
           .NotEmpty()
           .WithMessage(ValidationConsts.EMPTY_JOB_ERROR);
        ;

        RuleFor(p => p.WorkExperience)//стаж*
           .NotNull()
           .WithMessage(ValidationConsts.EMPTY_WORKEXPERIANCE_ERROR)
           .NotEmpty()
           .WithMessage(ValidationConsts.EMPTY_WORKEXPERIANCE_ERROR)
           .Matches("^(?:[1-9]?[0-9])$")
           .WithMessage(ValidationConsts.NOT_VALID_WORKEXPERIANCE_ERROR);
        ;

        RuleFor(p => p.UserSkills)//навыки* IEnumerable
            ;

        RuleFor(p => p.UserIntents)//цели* IEnumerable
            ;
    }
}