namespace LeokaEstetica.Platform.Core.Constants;

/// <summary>
/// Класс описывает ключи приложения.
/// </summary>
public static class GlobalConfigKeys
{
    /// <summary>
    /// Если не передали аккаунт пользователя.
    /// </summary>
    public const string EMPTY_ACCOUNT = "Не передан аккаунт пользователя.";
    
    /// <summary>
    /// Класс описывает ключи для кэшей приложения.
    /// </summary>
    public static class Cache
    {
        /// <summary>
        /// Ключ для меню профиля пользователя.
        /// </summary>
        public const string PROFILE_MENU_KEY = "ProfileMenu";

        /// <summary>
        /// Ключ для меню вакансий.
        /// </summary>
        public const string VACANCY_MENU_KEY = "VacancyMenu";

        /// <summary>
        /// Ключ для исключения полей при валидации.
        /// </summary>
        public const string VALIDATION_EXCLUDE_KEY = "ValidationExclude";
    }
    
    /// <summary>
    /// Класс описывает ключи для валидации проектов.
    /// </summary>
    public static class ProjectMode
    {
        /// <summary>
        /// Если не заполнили название проекта.
        /// </summary>
        public const string EMPTY_PROJECT_NAME = "Не заполнено название проекта.";
    
        /// <summary>
        /// Если не заполнили описание проекта.
        /// </summary>
        public const string EMPTY_PROJECT_DETAILS = "Не заполнено описание проекта.";

        /// <summary>
        /// Если передали некорректный Id проекта.
        /// </summary>
        public const string NOT_VALID_PROJECT_ID = "Некорректный Id проекта. ProjectId был ";

        /// <summary>
        /// Если передали некорректный режим.
        /// </summary>
        public const string EMPTY_MODE = "Передан некорректный режим. Mode был ";
    }

    /// <summary>
    /// Класс описывает ключи валидации вакансий.
    /// </summary>
    public static class Vacancy
    {
        /// <summary>
        /// Если не заполнили название вакансии.
        /// </summary>
        public const string EMPTY_VACANCY_NAME = "Название вакансии не может быть пустым.";
    
        /// <summary>
        /// Если не заполнили описание вакансии.
        /// </summary>
        public const string EMPTY_VACANCY_TEXT = "Описание вакансии не может быть пустым.";
    }

    /// <summary>
    /// Класс описывает ключи для валидации вакансий проектов.
    /// </summary>
    public static class ProjectVacancy
    {
        /// <summary>
        /// Если не заполнили название вакансии проекта.
        /// </summary>
        public const string EMPTY_PROJECT_VACANCY_NAME = "Не заполнено название вакансии проекта.";

        /// <summary>
        /// Если не заполнили описание вакансии проекта.
        /// </summary>
        public const string EMPTY_PROJECT_VACANCY_TEXT = "Не заполнено описание вакансии проекта.";

        /// <summary>
        /// Если к проекту уже прикреплена такая вакансия.
        /// </summary>
        public const string DUBLICATE_PROJECT_VACANCY = "Вакансия уже прикреплена к проекту.";
    }
}