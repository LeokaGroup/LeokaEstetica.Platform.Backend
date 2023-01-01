using AutoMapper;
using LeokaEstetica.Platform.Models.Dto.Chat.Output;
using LeokaEstetica.Platform.Models.Dto.Output.Configs;
using LeokaEstetica.Platform.Models.Dto.Output.Header;
using LeokaEstetica.Platform.Models.Dto.Output.Landing;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Profile;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Dto.Output.User;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Models.Entities.Common;
using LeokaEstetica.Platform.Models.Entities.Communication;
using LeokaEstetica.Platform.Models.Entities.Configs;
using LeokaEstetica.Platform.Models.Entities.Landing;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.Profile;
using LeokaEstetica.Platform.Models.Entities.Project;
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
    }
}