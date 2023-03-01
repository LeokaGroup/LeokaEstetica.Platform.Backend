using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.Notification;

namespace LeokaEstetica.Platform.Controllers.Validators.Notification;

/// <summary>
/// Класс валидатора апрува инвайта в проект.
/// </summary>
public class ApproveProjectInviteValidator : AbstractValidator<ApproveProjectInviteInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public ApproveProjectInviteValidator()
    {
        RuleFor(p => p.NotificationId)
            .Must(p => p > 0)
            .WithMessage(GlobalConfigKeys.Notification.ProjectNotification.NEGATIVE_NOTIFICATION_ID);
    }
}