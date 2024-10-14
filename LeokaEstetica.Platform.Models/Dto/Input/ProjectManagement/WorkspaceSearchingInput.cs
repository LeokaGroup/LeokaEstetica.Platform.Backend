using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement
{
	/// <summary>
	/// Класс входной модели поиска в разделе "Мое пространство"
	/// </summary>
	public class WorkspaceSearchingInput
	{
		/// <summary>
		/// Поиск по id пространства
		/// </summary>
		public bool IsById { get; set; }

		/// <summary>
		/// Поиск по названию проекта
		/// </summary>
		public bool IsByProjectName { get; set; }

		/// <summary>
		/// Поиск по id проекта или по названию проекта 
		/// </summary>
		public string? SearchText { get; set; }

	}
}
