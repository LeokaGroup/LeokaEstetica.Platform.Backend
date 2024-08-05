using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

			RuleFor(p => p.IsPublic)
				.NotNull()
				.WithMessage(ValidationConst.ProjectValidation.EMPTY_BOOLEAN_FLAG);
		}
	}
}
