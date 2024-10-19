using FluentValidation.Results;
using LeokaEstetica.Platform.Models.Dto.Common;

namespace LeokaEstetica.Platform.Models.Dto.Communications.Output;

/// <summary>
/// Класс выходной модели создания диалога и добавления участников в диалог.
/// </summary>
public class CreateDialogAndAddDialogMembersOutput : IFrontError
{
    /// <summary>
    /// Признак успеха.
    /// </summary>
    public bool IsSuccess { get; set; }
    
    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<ValidationFailure>? Errors { get; set; }

    /// <summary>
    /// Созданный диалог.
    /// </summary>
    public GroupObjectDialogOutput? Dialog { get; set; }
}