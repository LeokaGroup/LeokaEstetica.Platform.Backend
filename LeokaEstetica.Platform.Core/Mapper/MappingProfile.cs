﻿using AutoMapper;
using LeokaEstetica.Platform.Core.Utils;
using LeokaEstetica.Platform.Models.Dto.Chat.Output;
using LeokaEstetica.Platform.Models.Dto.Common.Cache;
using LeokaEstetica.Platform.Models.Dto.Common.Cache.Output;
using LeokaEstetica.Platform.Models.Dto.Input.Moderation;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.YandexKassa;
using LeokaEstetica.Platform.Models.Dto.Output.Communication;
using LeokaEstetica.Platform.Models.Dto.Output.Configs;
using LeokaEstetica.Platform.Models.Dto.Output.Document;
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
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectTeam;
using LeokaEstetica.Platform.Models.Dto.Output.Refunds;
using LeokaEstetica.Platform.Models.Dto.Output.Resume;
using LeokaEstetica.Platform.Models.Dto.Output.Search.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Search.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.Subscription;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
using LeokaEstetica.Platform.Models.Dto.Output.Ticket;
using LeokaEstetica.Platform.Models.Dto.Output.User;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Models.Entities.Commerce;
using LeokaEstetica.Platform.Models.Entities.Common;
using LeokaEstetica.Platform.Models.Entities.Communication;
using LeokaEstetica.Platform.Models.Entities.Configs;
using LeokaEstetica.Platform.Models.Entities.Document;
using LeokaEstetica.Platform.Models.Entities.FareRule;
using LeokaEstetica.Platform.Models.Entities.Landing;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.Notification;
using LeokaEstetica.Platform.Models.Entities.Profile;
using LeokaEstetica.Platform.Models.Entities.Project;
using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using LeokaEstetica.Platform.Models.Entities.ProjectTeam;
using LeokaEstetica.Platform.Models.Entities.Subscription;
using LeokaEstetica.Platform.Models.Entities.Template;
using LeokaEstetica.Platform.Models.Entities.Ticket;
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

        CreateMap<ProjectVacancyEntity, ProjectVacancyOutput>()
            .ForMember(p => p.VacancyName,
                p => p.MapFrom(src => src.UserVacancy.VacancyName))
            .ForMember(p => p.VacancyText,
                p => p.MapFrom(src => src.UserVacancy.VacancyText))
            .ForMember(p => p.ProjectVacancyId,
                p => p.MapFrom(src => src.ProjectVacancyId));
        

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
        
        CreateMap<ProfileInfoEntity, ProfileInfoOutput>();
        
        CreateMap<FareRuleEntity, FareRuleOutput>();
        CreateMap<FareRuleItemEntity, FareRuleItemOutput>();

        CreateMap<OrderEntity, CreateOrderPayMasterOutput>();
        CreateMap<OrderEntity, CreateOrderYandexKassaOutput>();

        CreateMap<SubscriptionEntity, SubscriptionOutput>();
        
        CreateMap<ModerationResumeEntity, ResumeModerationOutput>();
        
        CreateMap<NotificationEntity, NotificationOutput>();
        
        CreateMap<TimelineEntity, TimelineOutput>();

        CreateMap<ArchivedProjectEntity, ProjectArchiveOutput>()
            .ForMember(a => a.ProjectName, a => a.MapFrom(src => src.UserProject.ProjectName));
        
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
        
        CreateMap<RefundEntity, RefundOutput>();
        
        CreateMap<RefundEntity, CreateRefundOutput>();
        
        CreateMap<ArchivedVacancyEntity, VacancyArchiveOutput>()
            .ForMember(a => a.VacancyName, a => a.MapFrom(src => src.UserVacancy.VacancyName));
        
        CreateMap<TicketCategoryEntity, TicketCategoryOutput>();
        
        CreateMap<MainInfoTicketEntity, TicketOutput>();
        
        CreateMap<TicketMessageEntity, TicketMessageOutput>();
        
        CreateMap<ProjectCommentEntity, LastProjectCommentsOutput>()
            .ForMember(a => a.Created, a => a.MapFrom(src => src.Created.ToString("g")));
        
        CreateMap<PlatformConditionEntity, PlatformConditionOutput>();
        
        CreateMap<ProjectCommentModerationEntity, ProjectCommentModerationOutput>()
            .ForMember(a => a.Created, a => a.MapFrom(src => src.ProjectComment.Created.ToString("g")))
            .ForMember(a => a.DateModeration, a => a.MapFrom(src => src.DateModeration.ToString("g")));
        
        CreateMap<DialogOutput, ProfileDialogOutput>();
        CreateMap<ProfileDialogOutput, DialogOutput>();
        
        CreateMap<ContactEntity, ContactOutput>();
        
        CreateMap<PublicOfferEntity, PublicOfferOutput>();
        
        CreateMap<UserEntity, ResumeModerationOutput>();
        
        CreateMap<CreateOrderCache, CreateOrderCacheOutput>();
        
        CreateMap<CreateOrderCacheOutput, OrderCacheOutput>();
        
        CreateMap<CreateRefundOutput, RefundOutput>();
        
        CreateMap<ViewStrategyEntity, ViewStrategyOutput>();
        
        CreateMap<PanelEntity, PanelOutput>();
        CreateMap<ProjectTaskEntity, ProjectManagmentTaskOutput>();
        CreateMap<ProjectTaskExtendedEntity, ProjectManagmentTaskOutput>();
        
        CreateMap<EpicEntity, ProjectManagmentTaskOutput>()
            .ForMember(p => p.Name, p => p.MapFrom(src => src.EpicName))
            .ForMember(p => p.AuthorId, p => p.MapFrom(src => src.CreatedBy))
            .ForMember(p => p.Details, p => p.MapFrom(src => src.EpicDescription))
            .ForMember(p => p.Updated, p => p.MapFrom(src => src.UpdatedAt ?? src.CreatedAt))
            .ForMember(p => p.ProjectTaskId, p => p.MapFrom(src => src.ProjectEpicId))
            .ForMember(p => p.ExecutorId, p => p.MapFrom(src => src.CreatedBy))
            .ForMember(p => p.Created, p => p.MapFrom(src => src.CreatedAt))
            .ForMember(p => p.TaskId, p => p.MapFrom(src => src.EpicId));
        
        CreateMap<UserStoryOutput, ProjectManagmentTaskOutput>()
            .ForMember(p => p.Name, p => p.MapFrom(src => src.StoryName))
            .ForMember(p => p.AuthorId, p => p.MapFrom(src => src.CreatedBy))
            .ForMember(p => p.Details, p => p.MapFrom(src => src.StoryDescription))
            .ForMember(p => p.Updated, p => p.MapFrom(src => src.UpdatedAt ?? src.CreatedAt))
            .ForMember(p => p.ProjectTaskId, p => p.MapFrom(src => src.UserStoryTaskId))
            .ForMember(p => p.ExecutorId, p => p.MapFrom(src => src.CreatedBy))
            .ForMember(p => p.Created, p => p.MapFrom(src => src.CreatedAt))
            .ForMember(p => p.TaskId, p => p.MapFrom(src => src.EpicId));

        CreateMap<ProjectManagmentTaskTemplateEntity, ProjectManagmentTaskTemplateOutput>();
        CreateMap<ProjectManagmentTaskStatusTemplateEntity, ProjectManagmentTaskStatusTemplateOutput>();
        CreateMap<ProjectManagmentTaskTemplateEntityResult, ProjectManagmentTaskTemplateResult>();
        CreateMap<TaskPriorityEntity, TaskPriorityOutput>();
        CreateMap<TaskTypeEntity, TaskTypeOutput>();
        CreateMap<ProjectTagEntity, ProjectTagOutput>();
        CreateMap<ProjectManagmentTaskStatusTemplateEntity, TaskStatusOutput>();
        
        CreateMap<ProfileInfoEntity, TaskPeopleOutput>()
            .ForMember(p => p.SecondName, p => p.MapFrom(src => src.Patronymic));
        
        CreateMap<ProjectDocumentEntity, ProjectTaskFileOutput>();
        
        CreateMap<ProjectTaskCommentEntity, TaskCommentOutput>();
        CreateMap<ProjectTaskCommentExtendedEntity, TaskCommentOutput>();
        
        CreateMap<EpicEntity, EpicOutput>();
        CreateMap<EpicEntity, AvailableEpicOutput>();
        
        CreateMap<UserStoryEntity, UserStoryOutput>();
        CreateMap<UserStoryStatusEntity, UserStoryStatusOutput>();
        
        CreateMap<ProjectManagementRoleRedis, ProjectManagementRoleOutput>();
        CreateMap<ProjectManagementRoleOutput, ProjectManagementRoleRedis>();
        CreateMap<ProjectManagmentTaskOutput, EpicTaskOutput>();
        CreateMap<ProjectManagmentTaskOutput, SearchAgileObjectOutput>();
        CreateMap<SearchAgileObjectOutput, ProjectManagmentTaskOutput>();
        CreateMap<EpicTaskOutput, ProjectManagmentTaskOutput>();
    }
    
    /// <summary>
    /// Метод мапит на абстриакцию.
    /// </summary>
    /// <typeparam name="TSource">Что маппим.</typeparam>
    /// <typeparam name="TDestination">На что маппим.</typeparam>
    private void CreateInterfaceMap<TSource, TDestination>() where TDestination : class
    {
        CreateMap<TSource, TDestination>().ConstructUsing(ctor: t => AutoFac.Resolve<TDestination>());
    }
}