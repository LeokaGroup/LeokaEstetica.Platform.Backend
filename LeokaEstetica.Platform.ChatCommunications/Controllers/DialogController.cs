using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Communications.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Communications.Input;
using LeokaEstetica.Platform.Models.Dto.Communications.Output;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Communications.Controllers;

/// <summary>
/// Контроллер диалогов чата.
/// </summary>
[AuthFilter]
[ApiController]
[Route("communications/dialogs")]
public class DialogController : BaseController
{
   private readonly IAbstractGroupDialogService _abstractGroupDialogService;
   
   /// <summary>
   /// Конструктор.
   /// </summary>
   /// <param name="abstractGroupDialogService">Сервис диалогов.</param>
   public DialogController(IAbstractGroupDialogService abstractGroupDialogService)
   {
      _abstractGroupDialogService = abstractGroupDialogService;
   }

   /// <summary>
   /// Метод создает диалог и добавляет в него участников.
   /// </summary>
   /// <param name="createDialogAndAddDialogMembers">Входная модель.</param>
   /// <returns>Выходная модель.</returns>
   [HttpPost]
   [Route("dialog")]
   [ProducesResponseType(200, Type = typeof(CreateDialogAndAddDialogMembersOutput))]
   [ProducesResponseType(400)]
   [ProducesResponseType(403)]
   [ProducesResponseType(500)]
   [ProducesResponseType(404)]
   public async Task<CreateDialogAndAddDialogMembersOutput> CreateDialogAndAddDialogMembersAsync(
      [FromBody] CreateDialogAndAddDialogMembers createDialogAndAddDialogMembers)
   {
      var result = await _abstractGroupDialogService.CreateDialogAndAddDialogMembersAsync(
         createDialogAndAddDialogMembers.MemberEmails, createDialogAndAddDialogMembers.DialogName, GetUserName(),
         createDialogAndAddDialogMembers.DialogGroupType, createDialogAndAddDialogMembers.AbstractId);

      return result;
   }

   [HttpPost]
   [Route("personal")]
   [ProducesResponseType(200, Type = typeof(CreateDialogAndAddDialogMembersOutput))]
   [ProducesResponseType(400)]
   [ProducesResponseType(403)]
   [ProducesResponseType(500)]
   [ProducesResponseType(404)]
   public async Task CreatePersonalDialogAsync([FromBody] CreatePersonalDialogInput createPersonalDialogInput)
   {
      
   }
}