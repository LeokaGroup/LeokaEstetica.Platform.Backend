using LeokaEstetica.Platform.Models.Dto.Base.Moderation.Output;

namespace LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

/// <summary>
/// Класс выходной модели замечаний вакансии.
/// </summary>
public class GetVacancyRemarkOutput : BaseGetRemarkOutput
{
    public GetVacancyRemarkOutput()
    {
        Severity = "warn";
    }
}