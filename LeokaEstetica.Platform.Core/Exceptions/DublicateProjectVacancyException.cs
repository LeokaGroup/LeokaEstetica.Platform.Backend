using LeokaEstetica.Platform.Core.Constants;

namespace LeokaEstetica.Platform.Core.Exceptions;

/// <summary>
/// Исключение дубликата вакансии, которая уже прикреплена к проекту.
/// </summary>
public class DublicateProjectVacancyException : Exception
{
    public DublicateProjectVacancyException() : base(ValidationConst.ProjectVacancy.DUBLICATE_PROJECT_VACANCY)
    {
    }
}