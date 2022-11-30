using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Utils;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

namespace LeokaEstetica.Platform.Services.Validators;

/// <summary>
/// Класс валидатора вакансий.
/// </summary>
public static class VacancyValidator
{
    /// <summary>
    /// Метод валидирует входные параметры при создании вакансии.
    /// </summary>
    /// <param name="result">Выходные данные. Писать ошибки валидации туда, если будут.</param>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="vacancyText">Описание вакансии.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    public static void ValidateCreateVacancy(ref CreateVacancyOutput result, string vacancyName, string vacancyText, string account)
    {
        var logger = AutoFac.Resolve<ILogService>();
        
        if (string.IsNullOrEmpty(vacancyName))
        {
            var ex = new ArgumentNullException(GlobalConfigKeys.Vacancy.EMPTY_VACANCY_NAME);
            result.Errors.Add(GlobalConfigKeys.Vacancy.EMPTY_VACANCY_NAME);
            logger.LogError(ex);
        }
        
        if (string.IsNullOrEmpty(vacancyText))
        {
            var ex = new ArgumentNullException(GlobalConfigKeys.Vacancy.EMPTY_VACANCY_TEXT);
            result.Errors.Add(GlobalConfigKeys.Vacancy.EMPTY_VACANCY_TEXT);
            logger.LogError(ex);
        }
        
        if (string.IsNullOrEmpty(account))
        {
            var ex = new ArgumentNullException($"Не передан аккаунт пользователя.");
            logger.LogError(ex);
        }
    }
}