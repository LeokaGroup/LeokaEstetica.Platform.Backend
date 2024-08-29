using LeokaEstetica.Platform.Models.Dto.Base.Commerce;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce;

namespace LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

/// <summary>
/// Класс выхродной модели расчета цены за публикацию вакансии.
/// </summary>
public class CalculatePostVacancyPriceOutput : BaseCalculatePrice
{
    /// <summary>
    /// Данные услуги.
    /// </summary>
    public FeesOutput? Fees { get; set; }
}