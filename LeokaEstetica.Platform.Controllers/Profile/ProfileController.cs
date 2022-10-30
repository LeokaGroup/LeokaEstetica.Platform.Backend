using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Core.Filters;
using LeokaEstetica.Platform.Models.Dto.Input.Profile;
using LeokaEstetica.Platform.Models.Dto.Output.Profile;
using LeokaEstetica.Platform.Services.Abstractions.Profile;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Profile;

/// <summary>
/// Контроллер профиля пользователя.
/// </summary>
[AuthFilter]
[ApiController]
[Route("profile")]
public class ProfileController : BaseController
{
    private readonly IProfileService _profileService;
    
    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    /// <summary>
    /// Метод получает основную информацию раздела обо мне.
    /// </summary>
    /// <returns>Данные раздела обо мне.</returns>
    [HttpGet]
    [Route("info")]
    [ProducesResponseType(200, Type = typeof(ProfileInfoOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ProfileInfoOutput> GetProfileInfoAsync()
    {
        var result = await _profileService.GetProfileInfoAsync(GetUserName());

        return result;
    }

    /// <summary>
    /// Метод получает список элементов меню профиля пользователя.
    /// </summary>
    /// <returns>Список элементов меню профиля пользователя.</returns>
    [HttpGet]
    [Route("menu")]
    [ProducesResponseType(200, Type = typeof(ProfileMenuItemsResultOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ProfileMenuItemsResultOutput> ProfileMenuItemsAsync()
    {
        var result = await _profileService.ProfileMenuItemsAsync();

        return result;
    }

    /// <summary>
    /// Метод получает список навыков для выбора в профиль пользователя.
    /// </summary>
    /// <returns>Список навыков.</returns>
    [HttpGet]
    [Route("skills")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<SkillOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<SkillOutput>> ProfileSkillsAsync()
    {
        var result = await _profileService.ProfileSkillsAsync();

        return result;
    }

    /// <summary>
    /// Метод получает список целей на платформе для выбора пользователем в профиль пользователя.
    /// </summary>
    /// <returns>Список целей.</returns>
    [HttpGet]
    [Route("intents")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<IntentOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<IntentOutput>> ProfileIntentsAsync()
    {
        var result = await _profileService.ProfileIntentsAsync();

        return result;
    }

    /// <summary>
    /// Метод сохраняет данные контактной информации пользователя.
    /// </summary>
    /// <param name="profileInfoInput">Входная модель.</param>
    /// <returns>Сохраненные данные.</returns>
    [HttpPost]
    [Route("info")]
    [ProducesResponseType(200, Type = typeof(ProfileInfoOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ProfileInfoOutput> SaveProfileInfoAsync([FromBody] ProfileInfoInput profileInfoInput)
    {
        var result = await _profileService.SaveProfileInfoAsync(profileInfoInput, GetUserName());

        return result;
    }

    /// <summary>
    /// Метод выбирает пункт меню профиля пользователя. Производит действия, если нужны. 
    /// </summary>
    /// <param name="selectMenuInput">Входная модель.</param>
    /// <returns>Системное название действия и роут если нужно.</returns>
    [HttpPost]
    [Route("select-menu")]
    [ProducesResponseType(200, Type = typeof(SelectMenuOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<SelectMenuOutput> SelectProfileMenuAsync([FromBody] SelectMenuInput selectMenuInput)
    {
        var result = await _profileService.SelectProfileMenuAsync(selectMenuInput.Text);

        return result;
    }
}