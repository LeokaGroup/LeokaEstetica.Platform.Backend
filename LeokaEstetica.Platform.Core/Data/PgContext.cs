using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Models.Entities.Access;
using LeokaEstetica.Platform.Models.Entities.Commerce;
using LeokaEstetica.Platform.Models.Entities.Common;
using LeokaEstetica.Platform.Models.Entities.Communication;
using LeokaEstetica.Platform.Models.Entities.Configs;
using LeokaEstetica.Platform.Models.Entities.FareRule;
using LeokaEstetica.Platform.Models.Entities.Landing;
using LeokaEstetica.Platform.Models.Entities.Logs;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.Notification;
using LeokaEstetica.Platform.Models.Entities.Profile;
using LeokaEstetica.Platform.Models.Entities.Project;
using LeokaEstetica.Platform.Models.Entities.ProjectTeam;
using LeokaEstetica.Platform.Models.Entities.Role;
using LeokaEstetica.Platform.Models.Entities.Subscription;
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
    /// Таблица логов.
    /// </summary>
    public DbSet<LogInfoEntity> LogInfos { get; set; }

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
}