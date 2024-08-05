using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeokaEstetica.Platform.Models.Dto.Output.Project
{
	public class UpdateVisibleProjectOutput
	{
		public long ProjectId { get; set; }
		public bool IsPublic { get; set; }
	}
}
