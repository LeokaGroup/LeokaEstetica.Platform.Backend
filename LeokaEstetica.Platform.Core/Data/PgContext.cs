using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Models.Entities.Common;
using LeokaEstetica.Platform.Models.Entities.Configs;
using LeokaEstetica.Platform.Models.Entities.Landing;
using LeokaEstetica.Platform.Models.Entities.Logs;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.Profile;
using LeokaEstetica.Platform.Models.Entities.Project;
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
}