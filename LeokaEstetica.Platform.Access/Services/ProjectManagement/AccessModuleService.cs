using LeokaEstetica.Platform.Access.Abstractions.ProjectManagement;
using LeokaEstetica.Platform.Access.Models.Output;
using LeokaEstetica.Platform.Database.Access.ProjectManagement;
using LeokaEstetica.Platform.Models.Enums;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Access.Services.ProjectManagement;

/// <summary>
/// Класс реализует методы сервиса проверки доступа к разным модулям платформы.
/// </summary>
internal sealed class AccessModuleService : IAccessModuleService
{
    private readonly IAccessModuleRepository _accessModuleRepository;
    private readonly ILogger<AccessModuleService> _logger;

    private const string AVAILABLE_FARE_RULE_ALL = "Функция доступна на тарифах \"Стартовый\", \"Основной\", " +
                                                   " \"Комплекстный\".";
    private const string AVAILABLE_FARE_RULE_MAIN = "Функция доступна на тарифах \"Основной\", \"Комплекстный\".";

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="accessModuleRepository">Репозиторий проверки доступов к модулям.</param>
    /// <param name="logger">Логгер..</param>
    public AccessModuleService(IAccessModuleRepository accessModuleRepository,
        ILogger<AccessModuleService> logger)
    {
        _accessModuleRepository = accessModuleRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<AccessProjectManagementOutput> CheckAccessProjectManagementModuleOrComponentAsync(long projectId,
        AccessModuleTypeEnum accessModule, AccessModuleComponentTypeEnum accessModuleComponentType)
    {
        try
        {
            var result = new AccessProjectManagementOutput();

            var isAccess = await _accessModuleRepository.CheckAccessProjectManagementModuleOrComponentAsync(projectId,
                accessModule, accessModuleComponentType);

            // Если нет доступа.
            if (!isAccess)
            {
                if (accessModule == AccessModuleTypeEnum.ProjectManagement)
                {
                    switch (accessModuleComponentType)
                    {
                        case AccessModuleComponentTypeEnum.ProjectTemplate:
                            result.ForbiddenTitle = "Шаблоны проекта";
                            result.ForbiddenText = "Шаблоны проектов позволяют скопировать набор полей проекта.";
                            result.FareRuleText = AVAILABLE_FARE_RULE_ALL;
                            break;

                        case AccessModuleComponentTypeEnum.ProjectTaskTemplate:
                            result.ForbiddenTitle = "Шаблоны задач проекта";
                            result.ForbiddenText = "Шаблоны задач проектов позволяют скопировать набор полей задачи " +
                                                   "проекта.";
                            result.FareRuleText = AVAILABLE_FARE_RULE_ALL;
                            break;
                        
                        case AccessModuleComponentTypeEnum.ProjectTaskFilter:
                            result.ForbiddenTitle = "Фильтры задач проекта";
                            result.ForbiddenText = "Фильтры задач проекта позволяют выполнять действия " +
                                                   "по фильтрации задач в рабочем пространстве по разным параметрам.";
                            result.FareRuleText = AVAILABLE_FARE_RULE_ALL;
                            break;
                            
                        case AccessModuleComponentTypeEnum.ProjectOnboarding:
                            result.ForbiddenTitle = "Онбординг проекта (регламенты)";
                            result.ForbiddenText = "Онбординг проекта представляет собой ознакомительную стартовую " +
                                                   "страницу в Wiki для новых участников команды проекта. " +
                                                   "Wiki проекта можно настраивать в настройках проекта.";
                            result.FareRuleText = AVAILABLE_FARE_RULE_ALL;
                            break;
                            
                        case AccessModuleComponentTypeEnum.EmployeeTime:
                            result.ForbiddenTitle = "Списание трудозатрат";
                            result.ForbiddenText = "Списание трудозатрат - списание затраченного сотрудником " +
                                                   "рабочего времени на выполнение задач. " +
                                                   "Открывает доступ к отчетам по трудозатратам сотрудников.";
                            result.FareRuleText = AVAILABLE_FARE_RULE_ALL;
                            break;
                        
                        case AccessModuleComponentTypeEnum.VirtualBoard:
                            result.ForbiddenTitle = "Виртуальная доска";
                            result.ForbiddenText = "Виртуальная доска позволяет проектировать схемы разных типов, " +
                                                   "сохранять схемы, экспортировать схемы в разные форматы.";
                            result.FareRuleText = AVAILABLE_FARE_RULE_MAIN;
                            break;
                            
                        // TODO: Дополнить описание.
                        case AccessModuleComponentTypeEnum.CreationRecruitStaff:
                            result.ForbiddenTitle = "Создание потребности на подбор сотрудников";
                            result.ForbiddenText = "";
                            result.FareRuleText = AVAILABLE_FARE_RULE_MAIN;
                            break;
                        
                        case AccessModuleComponentTypeEnum.GuestAccessProjectTask:
                            result.ForbiddenTitle = "Гостевой доступ к проекту и задачам";
                            result.ForbiddenText = "Гостевой доступ предоставляет приглашенным пользователям только " +
                                                   "право на чтение.";
                            result.FareRuleText = AVAILABLE_FARE_RULE_MAIN;
                            break;
                        
                        case AccessModuleComponentTypeEnum.GoogleCalendarIntegration:
                            result.ForbiddenTitle = "Интеграция с Google-календарем";
                            result.ForbiddenText = "Интеграция с Google-календарем предлагает возможность " +
                                                   "интеграции с Google-календарем. Благодаря этому можно видеть " +
                                                   "расписание сотрудников из Google-календаря в своих календарях.";
                            result.FareRuleText = AVAILABLE_FARE_RULE_MAIN;
                            break;
                            
                        case AccessModuleComponentTypeEnum.TelegramIntegration:
                            result.ForbiddenTitle = "Интеграция с Telegram";
                            result.ForbiddenText = "Интеграция с Telegram предлагает возможность " +
                                                   "получать уведомления о новых задачах и ключевых действиях прямо " +
                                                   "в своем Telegram.";
                            result.FareRuleText = AVAILABLE_FARE_RULE_MAIN;
                            break;
                        
                        case AccessModuleComponentTypeEnum.Reports:
                            result.ForbiddenTitle = "Отчеты";
                            result.ForbiddenText = "Доступ к отчетам по трудозатратам, аналитике и других типов " +
                                                   "отчетов с возможностью экспорта в разные форматы.";
                            result.FareRuleText = AVAILABLE_FARE_RULE_MAIN;
                            break;
                        
                        case AccessModuleComponentTypeEnum.Charts:
                            result.ForbiddenTitle = "Графики";
                            result.ForbiddenText = "Доступ к отчетам по трудозатратам, аналитике и других типов " +
                                                   "отчетов с возможностью экспорта в разные форматы.";
                            result.FareRuleText = AVAILABLE_FARE_RULE_MAIN;
                            break;

                        default:
                            throw new InvalidOperationException(
                                "При проверке доступов к модулям или компонентам не сработал ни один кейс.");
                    }
                }

                result.IsAccess = false;

                return result;
            }

            result.IsAccess = true;

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}