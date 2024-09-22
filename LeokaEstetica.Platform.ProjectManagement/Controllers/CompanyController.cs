using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Common.Cache;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.ProjectManagement;
using LeokaEstetica.Platform.ProjectManagement.Validators;
using LeokaEstetica.Platform.Redis.Abstractions.ProjectManagement;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.ProjectManagement.Controllers;

/// <summary>
/// Контроллер работы с компаниями.
/// </summary>
[ApiController]
[Route("project-management/companies")]
[AuthFilter]
public class CompanyController : BaseController
{
    private readonly ILogger<CompanyController> _logger;
    private readonly Lazy<IDiscordService> _discordService;
    private readonly ICompanyService _companyService;
    private readonly Lazy<ICompanyRepository> _companyRepository;
    private readonly IUserRepository _userRepository;
    private readonly Lazy<ICompanyRedisService> _companyRedisService;
    
    /// <summary>
    /// Конструктор.
    /// <param name="logger">Логгер.</param>
    /// <param name="discordService">Сервис уведомлений дискорда.</param>
    /// <param name="companyService">Сервис компаний.</param>
    /// <param name="companyRepository">Репозиторий компаний.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="companyRedisService">Сервис компаний в кэше.</param>
    /// </summary>
    public CompanyController(ILogger<CompanyController> logger,
        Lazy<IDiscordService> discordService,
        ICompanyService companyService,
        Lazy<ICompanyRepository> companyRepository,
        IUserRepository userRepository,
        Lazy<ICompanyRedisService> companyRedisService)
    {
        _logger = logger;
        _discordService = discordService;
        _companyService = companyService;
        _companyRepository = companyRepository;
        _userRepository = userRepository;
        _companyRedisService = companyRedisService;
    }

    /// <summary>
    /// Метод создает компанию.
    /// </summary>
    /// <param name="companyInput">Входная компания.</param>
    [HttpPost]
    [Route("company")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task CreateCompanyAsync([FromBody] CompanyInput companyInput)
    {
        var validator = await new CreateCompanyValidator().ValidateAsync(companyInput);

        if (validator.Errors.Any()) 
        {
            var exceptions = new List<InvalidOperationException>(); 

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }
            
            var ex = new AggregateException("Ошибка создания компании.", exceptions);
            _logger.LogError(ex, ex.Message);
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _companyService.CreateCompanyAsync(companyInput.CompanyName, GetUserName());
    }

    /// <summary>
    /// Метод вычисляет кол-во компаний пользователя.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("calculate-user-company")]
    [ProducesResponseType(200, Type = typeof(CalculateUserCompanyOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<CalculateUserCompanyOutput> CalculateUserCompanyAsync()
    {
        var userId = await _userRepository.GetUserByEmailAsync(GetUserName());

        if (userId == 0)
        {
            throw new InvalidOperationException($"Id пользователя с аккаунтом {GetUserName()} не найден.");
        }

        var calcCompaniesCount = await _companyRepository.Value.CalculateCountUserCompaniesByCompanyMemberIdAsync(
            userId);
        
        return new CalculateUserCompanyOutput
        {
            // Если компаний 0 - то требуем создать сначала компанию.
            // Если более 1, то требуем выбрать, к какой компании отнести проект.
            IsNeedUserAction = calcCompaniesCount is 0 or > 1,
            IfExistsAnyCompanies = calcCompaniesCount == 1,
            IfExistsMultiCompanies = calcCompaniesCount > 1
        };
    }

    /// <summary>
    /// Метод получает список компаний пользователя.
    /// </summary>
    /// <returns>Список компаний пользователя.</returns>
    [HttpGet]
    [Route("user-companies")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<CompanyOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<CompanyOutput>?> GetUserCompaniesAsync()
    {
        var userId = await _userRepository.GetUserByEmailAsync(GetUserName());

        if (userId == 0)
        {
            throw new InvalidOperationException($"Id пользователя с аккаунтом {GetUserName()} не найден.");
        }

        var result = await _companyRepository.Value.GetUserCompaniesAsync(userId) ?? Enumerable.Empty<CompanyOutput>();

        return result;
    }

    /// <summary>
    /// Метод добавляет компанию в кэш.
    /// </summary>
    /// <param name="companyInput">Входная модель.</param>
    [HttpPost]
    [Route("company-cache")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task CreateCompanyCacheAsync([FromBody] CompanyInput companyInput)
    {
        var userId = await _userRepository.GetUserByEmailAsync(GetUserName());

        if (userId == 0)
        {
            throw new InvalidOperationException($"Id пользователя с аккаунтом {GetUserName()} не найден.");
        }
        
        // TODO: Если будет много полей, то через маппер кастить.
        await _companyRedisService.Value.SetCompanyAsync(new CompanyRedis
        {
            CompanyName = companyInput.CompanyName,
            CreatedBy = userId
        });
    }
}