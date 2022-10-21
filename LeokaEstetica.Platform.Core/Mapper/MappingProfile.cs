using AutoMapper;
using LeokaEstetica.Platform.Models.Dto.Output.Header;
using LeokaEstetica.Platform.Models.Dto.Output.Landing;
using LeokaEstetica.Platform.Models.Dto.Output.Profile;
using LeokaEstetica.Platform.Models.Dto.Output.User;
using LeokaEstetica.Platform.Models.Entities.Common;
using LeokaEstetica.Platform.Models.Entities.Landing;
using LeokaEstetica.Platform.Models.Entities.Profile;
using LeokaEstetica.Platform.Models.Entities.User;

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
        CreateMap<SkillEntity, SkillOutput>();
        CreateMap<IntentEntity, IntentOutput>();
    }
}