using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Database.Chat;
using LeokaEstetica.Platform.Models.Dto.Chat.Output;

namespace LeokaEstetica.Platform.Messaging.Builders;

/// <summary>
/// Класс билдера сообщений.
/// </summary>
public static class CreateDialogMessagesBuilder
{
    /// <summary>
    /// Метод проводит необходимые операции с сообщениями.
    /// </summary>
    /// <param name="dialogs">Список диалогов.</param>
    /// <param name="chatRepository">Зависимость чата.</param>
    /// <param name="userRepository">Зависимость пользователя.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список диалогов.</returns>
    public static async Task<List<DialogOutput>> Create(List<DialogOutput> dialogs, IChatRepository chatRepository,
        IUserRepository userRepository, long userId)
    {
        foreach (var dialog in dialogs)
        {
            var lastMessage = await chatRepository.GetLastMessageAsync(dialog.DialogId);

            // Подтягиваем последнее сообщение для каждого диалога и проставляет после 40 символов ...
            if (lastMessage is not null)
            {
                dialog.LastMessage = lastMessage.Length > 40
                    ? string.Concat(lastMessage.Substring(0, 40), "...")
                    : lastMessage;
            }

            // Найдет Id участников диалога по DialogId.
            var membersIds = await chatRepository.GetDialogMembersAsync(dialog.DialogId);

            if (membersIds == null)
            {
                throw new InvalidOperationException($"Не найдено участников для диалога с DialogId {dialog.DialogId}");
            }

            // Записываем имя и фамилию участника диалога, с которым идет общение.
            var otherUserId = membersIds.FirstOrDefault(m => !m.Equals(userId));
            var otherData = await userRepository.GetUserByUserIdAsync(otherUserId);
            dialog.FullName = otherData.FirstName + " " + otherData.LastName;

            // Если дата диалога совпадает с сегодняшней, то заполнит часы и минуты, иначе оставит их null.
            if (DateTime.UtcNow.ToUniversalTime().ToString("d")
                .Equals(Convert.ToDateTime(dialog.Created).ToString("d")))
            {
                // Запишет только часы и минуты.
                dialog.CalcTime = Convert.ToDateTime(dialog.Created).ToString("t");
            }

            // Если дата диалога не совпадает с сегодняшней.
            else
            {
                // Записываем только дату.
                dialog.CalcShortDate = Convert.ToDateTime(dialog.Created).ToString("d");
            }

            // Форматируем дату убрав секунды.
            dialog.Created = Convert.ToDateTime(dialog.Created).ToString("g");
            
            var id = membersIds.Except(new[] { userId }).First();
            var user = await userRepository.GetUserByUserIdAsync(id);

            // Если имя и фамилия заполнены, то берем их.
            if (user.FirstName is not null 
                && user.LastName is not null)
            {
                dialog.FullName = user.FirstName + " " + user.LastName;
            }
            
            // Иначе берем почту.
            else
            {
                dialog.FullName = user.Email;
            }
        }

        return dialogs;
    }
}