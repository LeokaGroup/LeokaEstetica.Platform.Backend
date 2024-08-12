using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Output.Project;

namespace LeokaEstetica.Platform.Controllers.Validators.Project
{
	public class UpdateVisibleProjectValidator:AbstractValidator<UpdateVisibleProjectOutput>
	{
		public UpdateVisibleProjectValidator()
		{
			RuleFor(p => p.ProjectId)
				.NotEmpty()
				.WithMessage(ValidationConst.ProjectValidation.NOT_VALID_PROJECT_ID)
				.NotNull()
				.WithMessage(ValidationConst.ProjectValidation.NOT_VALID_PROJECT_ID);
		}
	}
}
