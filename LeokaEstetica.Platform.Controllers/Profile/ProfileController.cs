using FluentValidation.Results;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Controllers.Validators.Profile;
using LeokaEstetica.Platform.Models.Dto.Input.Profile;
using LeokaEstetica.Platform.Models.Dto.Output.Profile;
using LeokaEstetica.Platform.Services.Abstractions.Profile;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
    private readonly ILogger<ProfileController> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="profileService">Сервис профиля.</param>
    /// <param name="logger">Логгер.</param>
    public ProfileController(IProfileService profileService,
        ILogger<ProfileController> logger)
    {
        _profileService = profileService;
        _logger = logger;
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
        var result = new ProfileInfoOutput { Errors = new List<ValidationFailure>() };
        var validator = await new SaveProfileInfoValidator().ValidateAsync(profileInfoInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();
            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }
            
            var ex = new AggregateException(exceptions);
            _logger.LogError(ex, "Ошибки при попытке сохранения данных профиля.");
            
            result.Errors.AddRange(validator.Errors);

            return result;
        }
        
        result = await _profileService.SaveProfileInfoAsync(profileInfoInput, GetUserName());

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
    /// Метод получает выбранные навыки пользователя.
    /// </summary>
    /// <param name="userCode">Код пользователя.</param>
    /// <returns>Список навыков.</returns>
    [HttpGet]
    [Route("selected-skills")]
    [ProducesResponseType(200, Type = typeof(List<SkillInput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<List<SkillOutput>> GetSelectedUserSkillsAsync([FromQuery] Guid? userCode)
    {
        List<SkillOutput> result;

        if (userCode is null || userCode == Guid.Empty)
        {
            result = await _profileService.SelectedProfileUserSkillsAsync(null, GetUserName());
        }
        
        else
        {
            result = await _profileService.SelectedProfileUserSkillsAsync(userCode, GetUserName());   
        }

        return result;
    }
    
    /// <summary>
    /// Метод получает выбранные цели пользователя.
    /// </summary>
    /// <param name="userCode">Код пользователя.</param>
    /// <returns>Список целей.</returns>
    [HttpGet]
    [Route("selected-intents")]
    [ProducesResponseType(200, Type = typeof(List<IntentOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<List<IntentOutput>> GetSelectedUserIntentsAsync([FromQuery] Guid? userCode)
    {
        List<IntentOutput> result;

        if (userCode is null || userCode == Guid.Empty)
        {
            result = await _profileService.SelectedProfileUserIntentsAsync(null, GetUserName());
        }
        
        else
        {
            result = await _profileService.SelectedProfileUserIntentsAsync(userCode, GetUserName());   
        }

        return result;
    }
}