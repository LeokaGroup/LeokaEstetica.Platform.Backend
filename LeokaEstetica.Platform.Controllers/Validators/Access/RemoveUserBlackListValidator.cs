using FluentValidation;
using LeokaEstetica.Platform.CallCenter.Models.Dto.Input.Access;
using LeokaEstetica.Platform.Services.Consts;

namespace LeokaEstetica.Platform.Controllers.Validators.Access
{
    /// <summary>
    /// Класс валидатора удаления пользователя из ЧС.
    /// </summary>
    public class RemoveUserBlackListValidator : AbstractValidator<RemoveUserBlackListInput>
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        public RemoveUserBlackListValidator()
        {
            RuleFor(p => p.UserId)
                .Must(p => p > 0)
                .WithMessage(ValidationConsts.ACCESS_NOT_VALID_USER_ID);
        }
    }
}
