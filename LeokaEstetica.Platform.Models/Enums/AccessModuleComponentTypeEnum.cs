namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление типов компонентов модулей платформы для проверки доступа.
/// </summary>
public enum AccessModuleComponentTypeEnum
{
    /// <summary>
    /// Неизвестный тип.
    /// </summary>
    Undefined = 0,
    
    /// <summary>
    /// Шаблоны проекта.
    /// </summary>
    ProjectTemplate = 1,
    
    /// <summary>
    /// Шаблоны задач проекта.
    /// </summary>
    ProjectTaskTemplate = 2,
    
    /// <summary>
    /// Фильтры задач (доступ ко всем фильтрам в раб.пространстве проекта).
    /// </summary>
    ProjectTaskFilter = 3,
    
    /// <summary>
    /// Онбординг проекта.
    /// </summary>
    ProjectOnboarding = 4,
    
    /// <summary>
    /// Списание трудозатрат.
    /// </summary>
    EmployeeTime = 5,
    
    /// <summary>
    /// Виртуальная доска.
    /// </summary>
    VirtualBoard = 6,
    
    /// <summary>
    /// Создание потребности на подбор сотрудников.
    /// </summary>
    CreationRecruitStaff = 7,
    
    /// <summary>
    /// Гостевой доступ к проекту.
    /// </summary>
    GuestAccessProjectTask = 8,
    
    /// <summary>
    /// Интеграция с Google-календарь.
    /// </summary>
    GoogleCalendarIntegration = 9,
    
    /// <summary>
    /// Интеграция с телеграм.
    /// </summary>
    TelegramIntegration = 10
}