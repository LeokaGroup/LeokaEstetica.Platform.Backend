using System.ComponentModel.DataAnnotations;

namespace LeokaEstetica.Platform.Models.Dto.Base.Chat;

/// <summary>
/// Базовый класс выходной модели диалога.
/// </summary>
public class BaseDialogOutput
{
    /// <summary>
    /// Id диалога.
    /// </summary>
    public long DialogId { get; set; }
    
    /// <summary>
    /// Последнее сообщение в кратком виде для отображения в сокращенном виде диалога.
    /// </summary>
    [MaxLength(40)]
    public string LastMessage { get; set; }
    
    /// <summary>
    /// Вычисляемое время для диалогов.
    /// </summary>
    public string CalcTime { get; set; }
    
    /// <summary>
    /// Полная дата.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Id пользователя, который есть в диалоге.
    /// </summary>
    public long UserId { get; set; }
    
    /// <summary>
    /// Имя + фамилия.
    /// </summary>
    public string FullName { get; set; }
    
    /// <summary>
    /// Вычисляемая дата для диалогов.
    /// </summary>
    public string CalcShortDate { get; set; }
}