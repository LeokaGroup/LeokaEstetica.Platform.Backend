namespace LeokaEstetica.Platform.Models.Dto.Input.Vacancy
{
	/// <summary>
	/// Класс входной модели каталога вакансий с фильтрацией и пагинацией
	/// </summary>
	public class VacancyCatalogInput
	{
		/// <summary>
		/// Класс входной модели фильтрации вакансий.
		/// </summary>
		public FilterVacancyInput? Filters { get; set; }

		/// <summary>
		/// Id последней записи на фронте. Применяется дял пагинации.
		/// Если NULL, то отдаем 1 страницу каталога.
		/// </summary>
		public long? LastId { get; set; }

		/// <summary>
		/// TODO: Будет передаваться с фронта, не убирать пока.
		/// Кол-во записей в выборке.
		/// По дефолту 20.
		/// </summary>
		public short PaginationRows { get; set; }

		/// <summary>
		/// Поисковый текст.
		/// </summary>
		public string? SearchText { get; set; }
		
		/// <summary>
		/// Признак использования пагинации.
		/// </summary>
		public bool IsPagination { get; set; }
	}
}
