namespace LeokaEstetica.Platform.Database.Access.Ticket;

/// <summary>
/// Абстракция репозитория доступа тикетов.
/// </summary>
public interface IAccessTicketRepository
{
    /// <summary>
    /// Метод получает список ролей пользователя для доступа к тикетам.
    /// Вернет -1, если у пользователя вообще нет прав на доступ к тикетам в КЦ.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список ролей пользователя.</returns>
    Task<IEnumerable<int>> GetTicketUserRolesAsync(long userId);

    /// <summary>
    /// Метод проверяет, есть ли текущий пользователь в участниках тикета.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="ticketId">Id тикета.</param>
    /// <returns>Признак проверки.</returns>
    Task<bool> IfExistsTicketMemberAsync(long userId, long ticketId);
}