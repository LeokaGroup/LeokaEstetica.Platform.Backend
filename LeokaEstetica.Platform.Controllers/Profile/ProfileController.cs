using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Abstractions.Services.Validation;
using LeokaEstetica.Platform.Controllers.Filters;
using LeokaEstetica.Platform.Controllers.Validators.Profile;
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
    private readonly IValidationExcludeErrorsService _validationExcludeErrorsService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="profileService">Сервис профиля.</param>
    /// <param name="validationExcludeErrorsService">Сервис исключения параметров валидации.</param>
    public ProfileController(IProfileService profileService, 
        IValidationExcludeErrorsService validationExcludeErrorsService)
    {
        _profileService = profileService;
        _validationExcludeErrorsService = validationExcludeErrorsService;
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
    [ProducesResponseType(200, Type = typeof(List<SkillOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<List<SkillOutput>> ProfileSkillsAsync()
    {
        var result = await _profileService.ProfileSkillsAsync(GetUserName());

        return result;
    }

    /// <summary>
    /// Метод получает список целей на платформе для выбора пользователем в профиль пользователя.
    /// </summary>
    /// <returns>Список целей.</returns>
    [HttpGet]
    [Route("intents")]
    [ProducesResponseType(200, Type = typeof(List<IntentOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<List<IntentOutput>> ProfileIntentsAsync()
    {
        var result = await _profileService.ProfileIntentsAsync(GetUserName());

        return result;
    }

    /// <summary>
    /// Метод сохраняет данные анкеты пользователя.
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
        var result = new ProfileInfoOutput();
        var validator = await new SaveProfileInfoValidator().ValidateAsync(profileInfoInput);

        if (validator.Errors.Any())
        {
            result.Errors = await _validationExcludeErrorsService.ExcludeAsync(validator.Errors);

            return result;
        }
        
        result = await _profileService.SaveProfileInfoAsync(profileInfoInput, GetUserName(), CreateTokenFromHeader());

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

    /// <summary>
    /// Метод получает список выбранные навыки пользователя.
    /// </summary>
    /// <returns>Список навыков.</returns>
    [HttpGet]
    [Route("selected-skills")]
    [ProducesResponseType(200, Type = typeof(List<SkillInput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<List<SkillOutput>> GetSelectedUserSkillsAsync()
    {
        var result = await _profileService.SelectedProfileUserSkillsAsync(GetUserName());

        return result;
    }
    
    /// <summary>
    /// Метод получает список выбранные цели пользователя.
    /// </summary>
    /// <returns>Список целей.</returns>
    [HttpGet]
    [Route("selected-intents")]
    [ProducesResponseType(200, Type = typeof(List<IntentOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<List<IntentOutput>> GetSelectedUserIntentsAsync()
    {
        var result = await _profileService.SelectedProfileUserIntentsAsync(GetUserName());

        return result;
    }
}