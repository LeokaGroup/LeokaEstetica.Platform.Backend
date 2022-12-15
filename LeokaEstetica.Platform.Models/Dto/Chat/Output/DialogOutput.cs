using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LeokaEstetica.Platform.Models.Dto.Chat.Output;

/// <summary>
/// Класс выходной модели диалога.
/// </summary>
public class DialogOutput
{
    /// <summary>
    /// Id диалога.
    /// </summary>
    public long DialogId { get; set; }

    /// <summary>
    /// Название диалога (фамилия и имя с кем ведется переписка).
    /// </summary>
    public string DialogName { get; set; }

    /// <summary>
    /// Последнее сообщение в кратком виде для отображения в сокращенном виде диалога.
    /// </summary>
    [MaxLength(40)]
    public string LastMessage { get; set; }

    /// <summary>
    /// Имя собеседника.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Фамилия собеседника.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Вычисляемое время для диалогов.
    /// </summary>
    public string CalcTime { get; set; }

    /// <summary>
    /// Вычисляемая дата для диалогов.
    /// </summary>
    public string CalcShortDate { get; set; }

    /// <summary>
    /// Полная дата.
    /// </summary>
    public string Created { get; set; }

    /// <summary>
    /// Id пользователя, который есть в диалоге.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Имя + фамилия.
    /// </summary>
    public string FullName { get; set; }
}