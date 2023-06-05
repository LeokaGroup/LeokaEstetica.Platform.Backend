using AutoMapper;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Diagnostics.Abstractions.Metrics;
using LeokaEstetica.Platform.Diagnostics.Helpers;
using LeokaEstetica.Platform.Models.Dto.Output.Metrics;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Metrics;

/// <summary>
/// Контроллер работы с метриками.
/// </summary>
[ApiController]
[Route("metrics")]
public class MetricsController : BaseController
{
    private readonly IUserMetricsService _userMetricsService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="userMetricsService">Сервис метрик пользователей.</param>
    /// <param name="mapper">Автомаппер.</param>
    public MetricsController(IUserMetricsService userMetricsService, 
        IMapper mapper)
    {
        _userMetricsService = userMetricsService;
        _mapper = mapper;
    }

    /// <summary>
    /// Метод получает список новых пользователей за текущий месяц.
    /// </summary>
    /// <returns>Список новых пользователей.</returns>
    [HttpGet]
    [Route("new-users")]
    [ProducesResponseType(200, Type = typeof(NewUserMetricsResult))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<NewUserMetricsResult> GetNewUsersAsync()
    {
        var items = await _userMetricsService.GetNewUsersAsync();
        var result = new NewUserMetricsResult
        {
            NewUsers = _mapper.Map<IEnumerable<NewUserMetricsOutput>>(items)
        };

        result.NewUsers = UserMetricsHelper.CreateDisplayTextNewUserMetrics(result.NewUsers.ToList());

        return result;
    }
}