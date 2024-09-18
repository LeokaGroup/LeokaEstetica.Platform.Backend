using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.ProjectManagement.Validators;
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
    
    /// <summary>
    /// Конструктор.
    /// <param name="logger">Логгер.</param>
    /// <param name="discordService">Сервис уведомлений дискорда.</param>
    /// <param name="companyService">Сервис компаний.</param>
    /// </summary>
    public CompanyController(ILogger<CompanyController> logger,
        Lazy<IDiscordService> discordService,
        ICompanyService companyService)
    {
        _logger = logger;
        _discordService = discordService;
        _companyService = companyService;
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
}