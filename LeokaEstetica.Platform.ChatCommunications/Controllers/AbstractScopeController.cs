using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Communications.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Communications.Output;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Communications.Controllers;

/// <summary>
/// Контроллер абстрактных областей чата.
/// </summary>
[ApiController]
[Route("abstract")]
[AuthFilter]
public class AbstractScopeController : BaseController
{
   private readonly IAbstractScopeService _abstractScopeService;
   
   /// <summary>
   /// Конструктор.
   /// </summary>
   /// <param name="abstractScopeService">Сервис абстрактных областей чата.</param>
   public AbstractScopeController(IAbstractScopeService abstractScopeService)
   {
      _abstractScopeService = abstractScopeService;
   }

   /// <summary>
   /// TODO: Изменить на сокеты.
   /// Метод получает список абстрактных областей чата.
   /// Учитывается текущий пользователь.
   /// Текущий метод можно расширять новыми абстрактными областями.
   /// </summary>
   /// <returns>Список абстрактных областей чата.</returns>
   [HttpGet]
   [Route("scopes")]
   [ProducesResponseType(200, Type = typeof(IEnumerable<AbstractScopeOutput>))]
   [ProducesResponseType(400)]
   [ProducesResponseType(403)]
   [ProducesResponseType(500)]
   [ProducesResponseType(404)]
   public async Task<IEnumerable<AbstractScopeOutput>> GetAbstractScopesAsync()
   {
      var result = await _abstractScopeService.GetAbstractScopesAsync(GetUserName());

      return result;
   }
}