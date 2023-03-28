namespace LeokaEstetica.Platform.Core.Exceptions;

/// <summary>
/// Исключение возникает, если не удалось найти вакансию по ее Id.
/// </summary>
public class NotFoundVacancyByIdException : InvalidOperationException
{
    public NotFoundVacancyByIdException(long vacancyId) : base($"Вакансия с Id: {vacancyId} не найдена!")
    {
    }
}


