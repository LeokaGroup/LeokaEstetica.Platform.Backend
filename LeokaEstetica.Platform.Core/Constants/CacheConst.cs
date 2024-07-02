namespace LeokaEstetica.Platform.Core.Constants;

/// <summary>
/// Класс констант кэша.
/// </summary>
public static class CacheConst
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

        /// <summary>
        /// Ключ для исключения полей при валидации.
        /// </summary>
        public const string VALIDATION_EXCLUDE_KEY = "ValidationExclude";

        /// <summary>
        /// Ключ ролей пользователя в модуле УП.
        /// </summary>
        public const string PROJECT_MANAGEMENT_USER_ROLES = "ProjectManagementRoles:";
        
        /// <summary>
        /// Ключ для меню хидера.
        /// </summary>
        public const string HEADER_MENU_KEY = "HeaderItems";
    }
}