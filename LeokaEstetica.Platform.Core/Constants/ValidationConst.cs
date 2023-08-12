namespace LeokaEstetica.Platform.Core.Constants;

/// <summary>
/// Класс констант для валидаций.
/// </summary>
public static class ValidationConst
{
    /// <summary>
    /// Класс описывает ключи для валидации проектов.
    /// </summary>
    public static class ProjectValidation
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

        /// <summary>
        /// Если не передали список Id замечаний проекта.
        /// </summary>
        public const string EMPTY_REMARK_IDS = "Не передан список Id замечаний проекта.";
    }
    
    /// <summary>
    /// Класс описывает ключи поиска в проектах.
    /// </summary>
    public static class SearchProject
    {
        /// <summary>
        /// Если не заполнена поисковая строка.
        /// </summary>
        public const string NOT_EMPTY_SEARCH_TEXT = "Поисковая строка не может быть пустой.";

        /// <summary>
        /// Если превышена максимальная длина поискоsвой строки.
        /// </summary>
        public const string MAX_LENGHT_EXCEEDED = "Поисковая строка не может превышать больше 100 символов.";
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
    
    /// <summary>
    /// Класс описывает ключи для чата.
    /// </summary>
    public static class Chat
    {
        /// <summary>
        /// Если невалидный тип предмета обсуждения.
        /// </summary>
        public const string NOT_VALID_DISCUSSION_TYPE = "Тип предмета обсуждения невалидный.";

        /// <summary>
        /// Если пустой предмет обсуждения.
        /// </summary>
        public const string NOT_EMPTY_DISCUSSION_TYPE = "Тип предмета обсуждения не может быть пустым.";
    }
    
    /// <summary>
    /// Класс ключей откликов на проекты.
    /// </summary>
    public static class ProjectResponse
    {
        /// <summary>
        /// Если уже был оставлен отклик к проекту.
        /// </summary>
        public const string DUBLICATE_PROJECT_RESPONSE = "Вы уже откликались на этот проект.";
    }

    /// <summary>
    /// Класс валидации тикетов.
    /// </summary>
    public static class TicketValidation
    {
        /// <summary>
        /// Если не валидная категория тикетов.
        /// </summary>
        public const string NOT_VALID_CATEGORY_NAME = "Категория не может быть пустой.";

        /// <summary>
        /// Если не передали сообщение тикета.
        /// </summary>
        public const string EMPTY_MESSAGE = "Сообщение тикета не может быть пустым.";

        /// <summary>
        /// Если не передали Id тикета.
        /// </summary>
        public const string NOT_VALID_TICKET_ID = "Id тикета был <= 0.";
    }

    /// <summary>
    /// Класс валидации комментариев проекта.
    /// </summary>
    public static class ProjectCommentValidation
    {
        /// <summary>
        /// Если не передали Id комментария проекта.
        /// </summary>
        public const string NOT_VALID_COMMENT_ID = "Id комментария должен быть заполнен.";
    }
}