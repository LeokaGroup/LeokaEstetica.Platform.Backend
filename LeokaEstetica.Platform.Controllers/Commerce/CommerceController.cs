using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Core.Filters;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Commerce;

/// <summary>
/// Контроллер работы с коммерцией (платежной системой, платежами, чеками и т.д).
/// </summary>
[AuthFilter]
[ApiController]
[Route("commercial")]
public class CommerceController : BaseController
{
    /// <inheritdoc />
    public CommerceController()
    {
    }
}