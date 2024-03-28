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

    /// <summary>
    /// Класс валидации модуля УП (управление проектами).
    /// </summary>
    public static class ProjectManagmentValidation
    {
        /// <summary>
        /// Если передали невалидный Id проекта.
        /// </summary>
        public const string NOT_VALID_PROJECT_ID = "Id проекта не передан.";
        
        /// <summary>
        /// Если не передали стратегию представления.
        /// </summary>
        public const string EMPTY_STRATEGY = "Стратегия представления не передана.";
        
        /// <summary>
        /// Если передали невалидный Id шаблона.
        /// </summary>
        public const string NOT_VALID_TEMPLATE_ID = "Id шаблона не передан.";

        /// <summary>
        /// Если не заполнили название тега.
        /// </summary>
        public const string EMPTY_TAG_NAME = "Не заполнено название тега.";

        /// <summary>
        /// Если не заполнили название статуса.
        /// </summary>
        public const string EMPTY_STATUS_NAME = "Не заполнено название статуса.";

        /// <summary>
        /// Если не заполнили название статуса ассоциации.
        /// </summary>
        public const string ASSOCIATION_SYS_NAME = "Не заполнено название статуса ассоциации.";
        
        /// <summary>
        /// Если не заполнили название перехода.
        /// </summary>
        public const string EMPTY_TRANSITION_NAME = "Не заполнено название перехода.";
        
        /// <summary>
        /// Если передали невалидный Id задачи проекта.
        /// </summary>
        public const string NOT_VALID_PROJECT_TASK_ID = "Id задачи проекта не передан.";
        
        /// <summary>
        /// Если передали невалидный Id статуса задачи.
        /// </summary>
        public const string NOT_VALID_TASK_STATUS_ID = "Id статуса задачи не передан.";

        /// <summary>
        /// Если не передали Id тега задачи.
        /// </summary>
        public const string NOT_VALID_TASK_TAG_ID = "Id тега задачи не передан.";

        /// <summary>
        /// Если не передали Id задачи, от которой исходит связь не передана.
        /// </summary>
        public const string NOT_VALID_TASK_FROM_LINK = "Id задачи, от которой исходит связь не передана.";
        
        /// <summary>
        /// Если не передали Id задачи, которую связывают.
        /// </summary>
        public const string NOT_VALID_TASK_TO_LINK = "Id задачи, которую связывают не передали.";

        /// <summary>
        /// Если передали недопустимый тип связи.
        /// </summary>
        public const string NOT_VALID_TASK_LINK_ENUM_VALUE = "Значение не является допустимым типом связи.";

        /// <summary>
        /// Если не передали Id наблюдателя задачи.
        /// </summary>
        public const string NOT_VALID_TASK_WATCHER_ID = "Id наблюдателя задачи не передан.";

        /// <summary>
        /// Если не передали Id исполнителя задачи.
        /// </summary>
        public const string NOT_VALID_TASK_EXECUTOR_ID = "Id исполнителя задачи не передан.";

        /// <summary>
        /// Если не передали Id приоритета.
        /// </summary>
        public const string NOT_VALID_TASK_PRIORITY_ID = "Id приоритета задачи не передан.";
        
        /// <summary>
        /// Если передали невалидный Id задачи проекта для разрыва связи.
        /// </summary>
        public const string NOT_VALID_TASK_LINK_REMOVED_TASK_ID = "Id задачи проекта для разрыва связи не передан.";

        /// <summary>
        /// Если передали невалидный Id документа.
        /// </summary>
        public const string NOT_VALID_DOCUMENT_ID = "Id документа не передан.";

        /// <summary>
        /// Если не передали системное название представления.
        /// </summary>
        public const string NOT_VALID_STRATEGY_SYS_NAME = "Системное название представления не передано.";
        
        /// <summary>
        /// Если не передали название в управлении проектом.
        /// </summary>
        public const string NOT_VALID_PROJECT_MANAGEMENT_PROJECT_NAME = "Название в управлении проектом не передано.";
        
        /// <summary>
        /// Если не передали префикс названия в управлении проектом.
        /// </summary>
        public const string NOT_VALID_PROJECT_MANAGEMENT_PROJECT_NAME_PREFIX =
            "Префикс названия в управлении проектом не передано.";

        /// <summary>
        /// Если не передали комментарий задачи.
        /// </summary>
        public const string EMPTY_TASK_COMMENT = "Не передан комментарий задачи.";
        
        /// <summary>
        /// Если не передали Id комментария задачи.
        /// </summary>
        public const string EMPTY_TASK_COMMENT_ID = "Не передан Id комментария задачи.";

        /// <summary>
        /// Если не передали поисковый текст.
        /// </summary>
        public const string EMPTY_SEARCH_TEXT = "Поисковый текст не передан.";
        
        /// <summary>
        /// Если передали невалидный Id эпика.
        /// </summary>
        public const string NOT_VALID_EPIC_ID = "Id эпика не передан.";
        
        /// <summary>
        /// Если не передано название спринта.
        /// </summary>
        public const string NOT_VALID_SPRINT_NAME = "Не передано название спринта.";

        /// <summary>
        /// Если не передали тип детализации.
        /// </summary>
        public const string NOT_VALID_TASK_DETAIL_TYPE = "Не передан тип детализации.";
    }
}