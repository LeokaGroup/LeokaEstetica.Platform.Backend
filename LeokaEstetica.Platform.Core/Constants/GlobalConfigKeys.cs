namespace LeokaEstetica.Platform.Core.Constants;

/// <summary>
/// Класс описывает ключи приложения.
/// </summary>
public static class GlobalConfigKeys
{
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
}