using FluentValidation.Results;
using LeokaEstetica.Platform.Models.Dto.Common;

namespace LeokaEstetica.Platform.Models.Dto.Chat.Output;

/// <summary>
/// Класс с результатами диалогов.
/// </summary>
public class DialogResultOutput : IFrontError
{
    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<ValidationFailure> Errors { get; set; }

    /// <summary>
    /// Список сообщений диалога.
    /// </summary>
    public List<DialogMessageOutput> Messages { get; set; }

    /// <summary>
    /// Id диалога.
    /// </summary>
    public long DialogId { get; set; }
    
    /// <summary>
    /// Состояние диалога.
    /// </summary>
    public string DialogState { get; set; }
    
    /// <summary>
    /// Имя пользователя, с которым идет общение.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Фамилия пользователя с которым идет общение.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Полное имя пользователя с которым идет общение.
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// Название предмета чата.
    /// </summary>
    public string ChatItemName { get; set; }
    
    /// <summary>
    /// Дата начала диалога.
    /// </summary>
    public string DateStartDialog { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long? ProjectId { get; set; }
    
    /// <summary>
    /// Тип события диалогов.
    /// </summary>
    public string ActionType { get; set; }
}