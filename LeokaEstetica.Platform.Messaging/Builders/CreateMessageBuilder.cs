using System.Text;
using LeokaEstetica.Platform.Messaging.Models.Chat.Output;
using LeokaEstetica.Platform.Models.Dto.Chat.Output;
using LeokaEstetica.Platform.Models.Entities.User;

namespace LeokaEstetica.Platform.Messaging.Builders;

/// <summary>
/// Класс билдера строит сообщения диалога.
/// </summary>
public static class CreateMessageBuilder
{
    private static StringBuilder _fullNameBuilder = new();
    
    /// <summary>
    /// Метод форматирует дату создания сообщений к удобному виду для фронта.
    /// </summary>
    /// <param name="messages">Список сообщений.</param>
    public static void FormatMessageDateCreated(ref List<DialogMessageOutput> messages)
    {
        foreach (var item in messages)
        {
            item.Created = string.Format("{0:f}", item.Created);
        }
    }

    /// <summary>
    /// Метод помечает сообщения текущего пользователя.
    /// </summary>
    /// <param name="messages">Список сообщений.</param>
    /// <param name="userId">Id пользователя.</param>
    public static void RemarkFlagMessagesCurrentUser(ref List<DialogMessageOutput> messages, long userId)
    {
        foreach (var msg in messages)
        {
            msg.IsMyMessage = msg.UserId == userId;   
        }
    }

    /// <summary>
    /// Метод запишет ФИО пользователя, с которым идет общение в чате.
    /// <param name="result">Результирующая модель.</param>
    /// <param name="user">Данные пользователя.</param>
    /// </summary>
    public static void SetUserFullNameDialog(ref DialogResultOutput result, UserEntity user)
    {
        _fullNameBuilder.Clear();
        result.FirstName = user.FirstName;
        result.LastName = user.LastName;
        _fullNameBuilder.Append(user.FirstName);
        _fullNameBuilder.Append(' ');
        _fullNameBuilder.Append(user.LastName);
        result.FullName = _fullNameBuilder.ToString();
    }
}