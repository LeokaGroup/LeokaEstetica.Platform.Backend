using AutoMapper;
using LeokaEstetica.Platform.Models.Dto.Chat.Output;
using LeokaEstetica.Platform.Models.Dto.Common.Cache;
using LeokaEstetica.Platform.Models.Dto.Input.Moderation;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Output.Configs;
using LeokaEstetica.Platform.Models.Dto.Output.FareRule;
using LeokaEstetica.Platform.Models.Dto.Output.Header;
using LeokaEstetica.Platform.Models.Dto.Output.Landing;
using LeokaEstetica.Platform.Models.Dto.Output.Metrics;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Resume;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Notification;
using LeokaEstetica.Platform.Models.Dto.Output.Orders;
using LeokaEstetica.Platform.Models.Dto.Output.Profile;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectTeam;
using LeokaEstetica.Platform.Models.Dto.Output.Resume;
using LeokaEstetica.Platform.Models.Dto.Output.Search.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Subscription;
using LeokaEstetica.Platform.Models.Dto.Output.User;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Models.Entities.Commerce;
using LeokaEstetica.Platform.Models.Entities.Common;
using LeokaEstetica.Platform.Models.Entities.Communication;
using LeokaEstetica.Platform.Models.Entities.Configs;
using LeokaEstetica.Platform.Models.Entities.FareRule;
using LeokaEstetica.Platform.Models.Entities.Landing;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.Notification;
using LeokaEstetica.Platform.Models.Entities.Profile;
using LeokaEstetica.Platform.Models.Entities.Project;
using LeokaEstetica.Platform.Models.Entities.ProjectTeam;
using LeokaEstetica.Platform.Models.Entities.Subscription;
using LeokaEstetica.Platform.Models.Entities.User;
using LeokaEstetica.Platform.Models.Entities.Vacancy;

namespace LeokaEstetica.Platform.Core.Mapper;

/// <summary>
/// Класс конфигурации маппера.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<HeaderEntity, HeaderOutput>();

        CreateMap<FonEntity, LandingStartFonOutput>();

        CreateMap<PlatformOfferEntity, PlatformOfferOutput>();

        CreateMap<PlatformOfferItemsEntity, PlatformOfferItemsOutput>();

        CreateMap<UserEntity, UserSignUpOutput>();

        CreateMap<ProfileInfoEntity, ProfileInfoOutput>();

        CreateMap<ProfileMenuItemEntity, ProfileMenuItemsResultOutput>();

        CreateMap<VacancyMenuItemEntity, VacancyMenuItemsResultOutput>();

        CreateMap<SkillEntity, SkillOutput>();

        CreateMap<IntentEntity, IntentOutput>();

        CreateMap<CatalogProjectEntity, CreateProjectOutput>();

        CreateMap<UserProjectEntity, CreateProjectOutput>();
        CreateMap<UserProjectEntity, ProjectOutput>();
        CreateMap<UserProjectEntity, UserProjectOutput>();

        CreateMap<ProjectColumnNameEntity, ProjectColumnNameOutput>();

        CreateMap<ProjectVacancyColumnNameEntity, ProjectVacancyColumnNameOutput>();

        CreateMap<UserVacancyEntity, VacancyOutput>();
        CreateMap<UserVacancyEntity, UserVacancyOutput>();

        CreateMap<ProjectStageEntity, ProjectStageOutput>();

        CreateMap<ProjectVacancyEntity, ProjectVacancyOutput>();

        CreateMap<UserVacancyOutput, UserVacancyEntity>();

        CreateMap<VacancyOutput, CreateProjectVacancyOutput>();

        CreateMap<ProjectResponseEntity, ProjectResponseOutput>();

        CreateMap<DialogMessageEntity, DialogMessageOutput>();

        CreateMap<ModerationProjectEntity, ProjectModerationOutput>();

        CreateMap<ModerationVacancyEntity, VacancyModerationOutput>();

        CreateMap<ProjectCommentEntity, ProjectCommentOutput>();

        CreateMap<ProjectTeamColumnNameEntity, ProjectTeamColumnNameOutput>();

        CreateMap<UserEntity, SearchProjectMemberOutput>()
            .ForMember(p => p.DisplayName,
                p => p.MapFrom(src => !string.IsNullOrEmpty(src.Email) ? src.Email : src.Login))
            .ForMember(p => p.UserId, p => p.MapFrom(src => src.UserId));
        
        CreateMap<ProjectTeamMemberEntity, ProjectTeamMemberOutput>();
        
        CreateMap<ProfileInfoEntity, ResumeOutput>();
        
        CreateMap<FareRuleEntity, FareRuleOutput>();
        CreateMap<FareRuleItemEntity, FareRuleItemOutput>();

        CreateMap<OrderEntity, CreateOrderOutput>();
        
        CreateMap<SubscriptionEntity, SubscriptionOutput>();
        
        CreateMap<ModerationResumeEntity, ResumeModerationOutput>();
        
        CreateMap<NotificationEntity, NotificationOutput>();
        
        CreateMap<TimelineEntity, TimelineOutput>();

        CreateMap<ArchivedProjectEntity, ProjectArchiveOutput>()
            .ForMember(a => a.ProjectName, a => a.MapFrom(src => src.UserProject.ProjectName))
            .ForMember(a => a.ProjectDetails, a => a.MapFrom(src => src.UserProject.ProjectDetails));
        
        CreateMap<UserEntity, UserActivityMarkDeactivate>();
        CreateMap<UserActivityMarkDeactivate, UserEntity>();
        
        CreateMap<ProjectRemarkInput, ProjectRemarkEntity>();
        CreateMap<ProjectRemarkEntity, ProjectRemarkInput>();
        CreateMap<ProjectRemarkEntity, ProjectRemarkOutput>();

        CreateMap<ProjectRemarkEntity, GetProjectRemarkOutput>();
        
        CreateMap<VacancyRemarkInput, VacancyRemarkEntity>();
        CreateMap<VacancyRemarkEntity, VacancyRemarkInput>();
        CreateMap<VacancyRemarkEntity, VacancyRemarkOutput>();
        
        CreateMap<VacancyRemarkEntity, GetVacancyRemarkOutput>();
        
        CreateMap<ResumeRemarkInput, ResumeRemarkEntity>();
        CreateMap<ResumeRemarkEntity, ResumeRemarkInput>();
        CreateMap<ResumeRemarkEntity, ResumeRemarkOutput>();
        
        CreateMap<ResumeRemarkEntity, GetResumeRemarkOutput>();

        CreateMap<UserProjectEntity, ProjectRemarkTableOutput>();
        
        CreateMap<UserVacancyEntity, VacancyRemarkTableOutput>();

        CreateMap<ProfileInfoEntity, ResumeRemarkTableOutput>()
            .ForMember(e => e.ProfileName,
                e => e.MapFrom(src => src.FirstName + " " + src.LastName + " " + src.Patronymic));
        
        CreateMap<CreateOrderCache, OrderCacheOutput>();
        
        CreateMap<OrderEntity, OrderOutput>();
        
        CreateMap<HistoryEntity, HistoryOutput>();
        
        CreateMap<UserEntity, NewUserMetricsOutput>();
    }
}