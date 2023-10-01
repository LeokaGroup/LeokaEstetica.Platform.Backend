using LeokaEstetica.Platform.Models.Entities.Communication;

namespace LeokaEstetica.Platform.Models.Entities.Vacancy;

/// <summary>
/// Класс сопоставляется с таблицей каталога вакансий Vacancies.CatalogVacancies.
/// </summary>
public class CatalogVacancyEntity
{
    public CatalogVacancyEntity()
    {
        MainInfoDialog = new HashSet<MainInfoDialogEntity>();
    }

    /// <summary>
    /// PK.
    /// </summary>
    public long CatalogVacancyId { get; set; }

    /// <summary>
    /// Id вакансии.
    /// </summary>
    public long VacancyId { get; set; }

    /// <summary>
    /// FK.
    /// </summary>
    public UserVacancyEntity Vacancy { get; set; }

    /// <summary>
    /// Информация о диалоге.
    /// </summary>
    public ICollection<MainInfoDialogEntity> MainInfoDialog { get; set; }
}