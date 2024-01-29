using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Models.Entities.Access;
using LeokaEstetica.Platform.Models.Entities.Commerce;
using LeokaEstetica.Platform.Models.Entities.Common;
using LeokaEstetica.Platform.Models.Entities.Communication;
using LeokaEstetica.Platform.Models.Entities.Configs;
using LeokaEstetica.Platform.Models.Entities.FareRule;
using LeokaEstetica.Platform.Models.Entities.Knowlege;
using LeokaEstetica.Platform.Models.Entities.Landing;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.Notification;
using LeokaEstetica.Platform.Models.Entities.Profile;
using LeokaEstetica.Platform.Models.Entities.Project;
using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using LeokaEstetica.Platform.Models.Entities.ProjectTeam;
using LeokaEstetica.Platform.Models.Entities.Role;
using LeokaEstetica.Platform.Models.Entities.Subscription;
using LeokaEstetica.Platform.Models.Entities.Template;
using LeokaEstetica.Platform.Models.Entities.Ticket;
using LeokaEstetica.Platform.Models.Entities.User;
using LeokaEstetica.Platform.Models.Entities.Vacancy;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Core.Data;

/// <summary>
/// Класс датаконтекста Postgres.
/// </summary>
public class PgContext : DbContext
{
    private readonly DbContextOptions<PgContext> _options;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="options"></param>
    public PgContext(DbContextOptions<PgContext> options) 
        : base(options)
    {
        _options = options;
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настраиваем все маппинги приложения.
        MappingsExtensions.Configure(modelBuilder);
    }
    
    /// <summary>
    /// Таблица фона.
    /// </summary>
    public DbSet<FonEntity> Fons { get; set; }

    /// <summary>
    /// Таблица хидера.
    /// </summary>
    public DbSet<HeaderEntity> Header { get; set; }

    /// <summary>
    /// Таблица предложений платформы.
    /// </summary>
    public DbSet<PlatformOfferEntity> PlatformOffer { get; set; }

    /// <summary>
    /// Таблица элементов предложений платформы.
    /// </summary>
    public DbSet<PlatformOfferItemsEntity> PlatformOfferItems { get; set; }

    /// <summary>
    /// Таблица пользователей.
    /// </summary>
    public DbSet<UserEntity> Users { get; set; }

    /// <summary>
    /// Таблица обо мне.
    /// </summary>
    public DbSet<ProfileInfoEntity> ProfilesInfo { get; set; }

    /// <summary>
    /// Таблица навыков пользователя.
    /// </summary>
    public DbSet<SkillEntity> Skills { get; set; }

    /// <summary>
    /// Таблица навыков, которые выбрал пользователь.
    /// </summary>
    public DbSet<UserSkillEntity> UserSkills { get; set; }

    /// <summary>
    /// Таблица целей на платформе, которые может выбрать пользователь.
    /// </summary>
    public DbSet<IntentEntity> Intents { get; set; }

    /// <summary>
    /// Таблица целей на платформе, которые выбрал пользователь.
    /// </summary>
    public DbSet<UserIntentEntity> UserIntents { get; set; }

    /// <summary>
    /// Таблица элементов меню профиля пользователя.
    /// </summary>
    public DbSet<ProfileMenuItemEntity> ProfileMenuItems { get; set; }
    
    /// <summary>
    /// Таблица каталога проектов.
    /// </summary>
    public DbSet<CatalogProjectEntity> CatalogProjects { get; set; }

    /// <summary>
    /// Таблица названий столбцов в управляющей таблице проектов.
    /// </summary>
    public DbSet<ProjectColumnNameEntity> ProjectColumnsNames { get; set; }
    
    /// <summary>
    /// Таблица названий столбцов в управляющей таблице вакансий проектов.
    /// </summary>
    public DbSet<ProjectVacancyColumnNameEntity> ProjectVacancyColumnsNames { get; set; }

    /// <summary>
    /// Таблица проектов пользователя.
    /// </summary>
    public DbSet<UserProjectEntity> UserProjects { get; set; }

    /// <summary>
    /// Таблица статусов проектов.
    /// </summary>
    public DbSet<ProjectStatusEntity> ProjectStatuses { get; set; }

    /// <summary>
    /// Таблица модерации проектов.
    /// </summary>
    public DbSet<ModerationProjectEntity> ModerationProjects { get; set; }

    /// <summary>
    /// Таблица вакансий пользователей.
    /// </summary>
    public DbSet<UserVacancyEntity> UserVacancies { get; set; }

    /// <summary>
    /// Таблица каталога вакансий.
    /// </summary>
    public DbSet<CatalogVacancyEntity> CatalogVacancies { get; set; }

    /// <summary>
    /// Таблица меню вакансий.
    /// </summary>
    public DbSet<VacancyMenuItemEntity> VacancyMenuItems { get; set; }

    /// <summary>
    /// Таблица статусов вакансий.
    /// </summary>
    public DbSet<VacancyStatusEntity> VacancyStatuses { get; set; }

    /// <summary>
    /// Таблица модерации вакансий.
    /// </summary>
    public DbSet<ModerationVacancyEntity> ModerationVacancies { get; set; }

    /// <summary>
    /// Таблица стадий проекта.
    /// </summary>
    public DbSet<ProjectStageEntity> ProjectStages { get; set; }

    /// <summary>
    /// Таблица стадий проектов пользователей.
    /// </summary>
    public DbSet<UserProjectStageEntity> UserProjectsStages { get; set; }

    /// <summary>
    /// Таблица вакансий проекта.
    /// </summary>
    public DbSet<ProjectVacancyEntity> ProjectVacancies { get; set; }

    /// <summary>
    /// Таблица управления исключения полей при валидации.
    /// </summary>
    public DbSet<ValidationColumnExcludeEntity> ValidationColumnsExclude { get; set; }

    /// <summary>
    /// Таблица откликов на проекты.
    /// </summary>
    public DbSet<ProjectResponseEntity> ProjectResponses { get; set; }

    /// <summary>
    /// Таблица статусов откликов на проекты.
    /// </summary>
    public DbSet<ProjectResponseStatuseEntity> ProjectResponseStatuses { get; set; }

    /// <summary>
    /// Таблица диалогов.
    /// </summary>
    public DbSet<MainInfoDialogEntity> Dialogs { get; set; }

    /// <summary>
    /// Таблица сообщений диалога.
    /// </summary>
    public DbSet<DialogMessageEntity> DialogMessages { get; set; }

    /// <summary>
    /// Таблица участников диалога.
    /// </summary>
    public DbSet<DialogMemberEntity> DialogMembers { get; set; }

    /// <summary>
    /// Таблица комментариев к проектам.
    /// </summary>
    public DbSet<ProjectCommentEntity> ProjectComments { get; set; }

    /// <summary>
    /// Таблица статусов комментариев проектов.
    /// </summary>
    public DbSet<ProjectCommentStatuseEntity> ProjectCommentsStatuses { get; set; }

    /// <summary>
    /// Таблица статусов.
    /// </summary>
    public DbSet<ModerationStatusEntity> ModerationStatuses { get; set; }

    /// <summary>
    /// Таблица ролей модерации.
    /// </summary>
    public DbSet<ModerationRoleEntity> ModerationRoles { get; set; }

    /// <summary>
    /// Таблица ролей модерации пользователей.
    /// </summary>
    public DbSet<ModerationUserRoleEntity> ModerationUserRoles { get; set; }

    /// <summary>
    /// Таблица модерации комментариев проектов.
    /// </summary>
    public DbSet<ProjectCommentModerationEntity> ProjectCommentsModeration { get; set; }

    /// <summary>
    /// Таблица команд проектов.
    /// </summary>
    public DbSet<ProjectTeamEntity> ProjectsTeams { get; set; }

    /// <summary>
    /// Таблица вакансий команд проектов.
    /// </summary>
    public DbSet<ProjectTeamVacancyEntity> ProjectsTeamsVacancies { get; set; }

    /// <summary>
    /// Таблица участников проектов.
    /// </summary>
    public DbSet<ProjectTeamMemberEntity> ProjectTeamMembers { get; set; }

    /// <summary>
    /// Таблица названий столбцов команд проектов.
    /// </summary>
    public DbSet<ProjectTeamColumnNameEntity> ProjectTeamColumnNames { get; set; }

    /// <summary>
    /// Таблица правил тарифов.
    /// </summary>
    public DbSet<FareRuleEntity> FareRules { get; set; }

    /// <summary>
    /// Таблица элементов тарифов.
    /// </summary>
    public DbSet<FareRuleItemEntity> FareRuleItems { get; set; }

    /// <summary>
    /// Таблица заказов.
    /// </summary>
    public DbSet<OrderEntity> Orders { get; set; }

    /// <summary>
    /// Таблица чеков.
    /// </summary>
    public DbSet<ReceiptEntity> Receipts { get; set; }

    /// <summary>
    /// Таблица подписок.
    /// </summary>
    public DbSet<SubscriptionEntity> Subscriptions { get; set; }

    /// <summary>
    /// Таблица подписок пользователей.
    /// </summary>
    public DbSet<UserSubscriptionEntity> UserSubscriptions { get; set; }

    /// <summary>
    /// Таблица ЧС почты пользователей.
    /// </summary>
    public DbSet<UserEmailBlackListEntity> UserEmailBlackList { get; set; }
    
    /// <summary>
    /// Таблица ЧС номеров телефонов пользователей.
    /// </summary>
    public DbSet<UserPhoneBlackListEntity> UserPhoneBlackList { get; set; }

    /// <summary>
    /// Теневая таблица истории ЧС почты пользователей.
    /// </summary>
    public DbSet<UserEmailBlackListShadowEntity> UserEmailBlackListShadow { get; set; }

    /// <summary>
    /// Теневая таблица истории ЧС номеров телефонов пользователей.
    /// </summary>
    public DbSet<UserPhoneBlackListShadowEntity> UserPhoneBlackListShadow { get; set; }

    /// <summary>
    /// Таблица модерации анкет.
    /// </summary>
    public DbSet<ModerationResumeEntity> ModerationResumes { get; set; }

    /// <summary>
    /// Таблица уведомлений.
    /// </summary>
    public DbSet<NotificationEntity> Notifications { get; set; }

    /// <summary>
    /// Таблица таймлайнов.
    /// </summary>
    public DbSet<TimelineEntity> Timelines { get; set; }

    /// <summary>
    /// Стартовая таблица БЗ.
    /// </summary>
    public DbSet<KnowledgeStartEntity> KnowledgeStart { get; set; }

    /// <summary>
    /// Таблица категорий.
    /// </summary>
    public DbSet<KnowledgeCategoryEntity> KnowledgeCategories { get; set; }

    /// <summary>
    /// Таблица подкатегорий.
    /// </summary>
    public DbSet<KnowledgeSubCategoryEntity> KnowledgeSubCategories { get; set; }

    /// <summary>
    /// Таблица тем подкатегории.
    /// </summary>
    public DbSet<KnowledgeSubCategoryThemeEntity> KnowledgeSubCategoryThemes { get; set; }

    /// <summary>
    /// Таблица ЧС VK.
    /// </summary>
    public DbSet<UserVkBlackListEntity> UserVkBlackList { get; set; }

    /// <summary>
    /// Теневая таблица ЧС VK.
    /// </summary>
    public DbSet<UserVkBlackListShadowEntity> UserVkBlackListShadow { get; set; }

    /// <summary>
    /// Таблица архива проектов.
    /// </summary>
    public DbSet<ArchivedProjectEntity> ArchivedProjects { get; set; }
    
    /// <summary>
    /// Таблица архива вакансий.
    /// </summary>
    public DbSet<ArchivedVacancyEntity> ArchivedVacancies { get; set; }

    /// <summary>
    /// Таблица глобал конфига.
    /// </summary>
    public DbSet<GlobalConfigEntity> GlobalConfig { get; set; }

    /// <summary>
    /// Таблица пользователей с доступом к КЦ.
    /// </summary>
    public DbSet<ModerationUserEntity> ModerationUsers { get; set; }

    /// <summary>
    /// Таблица статусов замечаний.
    /// </summary>
    public DbSet<RemarkStatuseEntity> RemarkStatuses { get; set; }

    /// <summary>
    /// Таблица замечаний проекта.
    /// </summary>
    public DbSet<ProjectRemarkEntity> ProjectRemarks { get; set; }

    /// <summary>
    /// Список замечаний вакансий.
    /// </summary>
    public DbSet<VacancyRemarkEntity> VacancyRemarks { get; set; }

    /// <summary>
    /// Таблица замечаний анкет.
    /// </summary>
    public DbSet<ResumeRemarkEntity> ResumeRemarks { get; set; }

    /// <summary>
    /// TODO: Проверить, будет ли использоваться вообще.
    /// TODO: Если нет, то удалить и из БД тоже.
    /// Таблица сервисов и услуг.
    /// </summary>
    public DbSet<ProductEntity> Products { get; set; }

    /// <summary>
    /// Таблица правил скидок.
    /// </summary>
    public DbSet<DiscountRuleEntity> DiscountRules { get; set; }

    /// <summary>
    /// Таблица списка транзакций по заказам пользователя.
    /// </summary>
    public DbSet<HistoryEntity> OrderTransactionsShadow { get; set; }

    /// <summary>
    /// Таблица возвратов.
    /// </summary>
    public DbSet<RefundEntity> Refunds { get; set; }

    /// <summary>
    /// Таблица основной информации тикетов.
    /// </summary>
    public DbSet<MainInfoTicketEntity> MainInfoTickets { get; set; }

    /// <summary>
    /// Участники тикета.
    /// </summary>
    public DbSet<TicketMemberEntity> TicketMembers { get; set; }

    /// <summary>
    /// Файлы тикета.
    /// </summary>
    public DbSet<TicketFileEntity> TicketFiles { get; set; }

    /// <summary>
    /// Сообщения тикета.
    /// </summary>
    public DbSet<TicketMessageEntity> TicketMessages { get; set; }

    /// <summary>
    /// Роли тикета.
    /// </summary>
    public DbSet<TicketRoleEntity> TicketRoles { get; set; }

    /// <summary>
    /// Статусы тикетов.
    /// </summary>
    public DbSet<TicketStatusEntity> TicketStatuses { get; set; }

    /// <summary>
    /// Роли пользователя тикета.
    /// </summary>
    public DbSet<UserTicketRoleEntity> UserTicketRoles { get; set; }

    /// <summary>
    /// Категории тикета.
    /// </summary>
    public DbSet<TicketCategoryEntity> TicketCategories { get; set; }

    /// <summary>
    /// Преимущества платформы.
    /// </summary>
    public DbSet<PlatformConditionEntity> PlatformConditions { get; set; }

    /// <summary>
    /// Таблица пожеланий/предложений.
    /// </summary>
    public DbSet<WisheOfferEntity> WishesOffers { get; set; }

    /// <summary>
    /// Таблица контактов.
    /// </summary>
    public DbSet<ContactEntity> Contacts { get; set; }

    /// <summary>
    /// Таблица публичной оферты.
    /// </summary>
    public DbSet<PublicOfferEntity> PublicOffer { get; set; }

    /// <summary>
    /// Таблица стратегий представления рабочего пространства проектов.
    /// </summary>
    public DbSet<ViewStrategyEntity> ViewStrategies { get; set; }

    /// <summary>
    /// Таблица элементов меню хидера модуля УП.
    /// </summary>
    public DbSet<ProjectManagmentHeaderEntity> ProjectManagmentHeader { get; set; }

    /// <summary>
    /// Таблица шаблонов задач. Содержит в себе шаблоны, которые касаются только задач (поддерживает и Kanban и Scrum).
    /// По сути, это набор столбцов в рабочем пространстве.
    /// Каждый столбец - это отдельный статус линии задач (вертикальный столбец).
    /// </summary>
    public DbSet<ProjectManagmentTaskTemplateEntity> ProjectManagmentTaskTemplates { get; set; }

    /// <summary>
    /// Таблица шаблонов статусов задач.
    /// </summary>
    public DbSet<ProjectManagmentTaskStatusTemplateEntity> ProjectManagmentTaskStatusTemplates { get; set; }

    /// <summary>
    /// Таблица шаблонов, которые выбрал пользователь.
    /// </summary>
    public DbSet<ProjectManagmentUserTaskTemplateEntity> ProjectManagmentUserTaskTemplates { get; set; }

    /// <summary>
    /// Таблица статусов многие-многие.
    /// </summary>
    public DbSet<ProjectManagmentTaskStatusIntermediateTemplateEntity> ProjectManagmentTaskStatusIntermediateTemplates
    {
        get;
        set;
    }

    /// <summary>
    /// Таблица задач проекта.
    /// </summary>
    public DbSet<ProjectTaskEntity> ProjectTasks { get; set; }

    /// <summary>
    /// Таблица отношений между задачами.
    /// </summary>
    public DbSet<TaskRelationEntity> TaskRelations { get; set; }

    /// <summary>
    /// Таблица зависимостей задач.
    /// </summary>
    public DbSet<TaskDependencyEntity> TaskDependencies { get; set; }

    /// <summary>
    /// Таблица резолюций задач.
    /// </summary>
    public DbSet<TaskResolutionEntity> TaskResolutions { get; set; }

    /// <summary>
    /// Таблица типов задач.
    /// </summary>
    public DbSet<TaskTypeEntity> TaskTypes { get; set; }

    /// <summary>
    /// Таблица комментариев к задачам.
    /// </summary>
    public DbSet<TaskCommentEntity> TaskComments { get; set; }

    /// <summary>
    /// История действий над задачей.
    /// </summary>
    public DbSet<TaskHistoryEntity> TaskHistories { get; set; }

    /// <summary>
    /// Таблица действий.
    /// </summary>
    public DbSet<HistoryActionEntity> HistoryActions { get; set; }

    /// <summary>
    /// Таблица приоритетов.
    /// </summary>
    public DbSet<TaskPriorityEntity> TaskPriorities { get; set; }

    /// <summary>
    /// Таблица настроек рабочего пространства проектов.
    /// </summary>
    public DbSet<ConfigSpaceSettingEntity> ConfigSpaceSettings { get; set; }

    /// <summary>
    /// Таблица кастомных статусов шаблонов пользователя.
    /// </summary>
    public DbSet<ProjectManagementUserStatuseTemplateEntity> ProjectManagementUserStatuseTemplates { get; set; }

    /// <summary>
    /// Таблица переходов статусов шаблонов.
    /// </summary>
    public DbSet<ProjectManagementTransitionTemplateEntity> ProjectManagementTransitionTemplates { get; set; }

    /// <summary>
    /// Таблица переходов статусов шаблонов пользователя.
    /// </summary>
    public DbSet<ProjectManagementUserTransitionTemplateEntity> ProjectManagementUserTransitionTemplates { get; set; }
    
    /// <summary>
    /// Таблица переходов многие-многие.
    /// </summary>
    public DbSet<ProjectManagementTransitionIntermediateTemplateEntity> ProjectManagementTransitionIntermediateTemplates
    {
        get;
        set;
    }
}