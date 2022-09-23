using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Landing;

[ApiController, Route("template")]
public class LandingController : BaseController
{
    // private  readonly BaseLogService<LandingController> _logger;
    //
    // public LandingController(BaseLogService<LandingController> logger)
    // {
    //     _logger = logger;
    // }

    [HttpGet]
    [Route("test")]
    public async Task Test()
    {
        try
        {
            throw new ArgumentNullException();
        }
        catch (Exception ex)
        {
            // await _logger.LogInfoAsync(ex, "sierra_93@mail.ru", LogLevelEnum.Error);
            throw;
        }
    }
}