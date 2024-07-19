using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace LeokaEstetica.Platform.Core.Exceptions
{
	public class NotFoundResumeByIdException: InvalidOperationException
	{
		public NotFoundResumeByIdException(long resumeId) : base($"Резюме с Id: {resumeId} не найден!")
		{

		}
	}
}
