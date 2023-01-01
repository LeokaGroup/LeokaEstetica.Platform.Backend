using System.Globalization;
using LeokaEstetica.Platform.Models.Entities.User;

namespace LeokaEstetica.Platform.Services.Builders;

/// <summary>
/// Класс билдера участников команды проекта.
/// </summary>
public static class CreateProjectTeamMembersBuilder
{
    /// <summary>
    /// Метод форматирует дату к нужному виду.
    /// </summary>
    /// <param name="date">Дата, которую нужно форматировать.</param>
    /// <returns>Дата в нужном виде.</returns>
    public static string Create(DateTime date)
    {
        return date.ToString("g", CultureInfo.GetCultureInfo("ru"));
    }

    /// <summary>
    /// Метод записывает участника команды проекта.
    /// </summary>
    /// <param name="user">Данные пользователя.</param>
    /// <returns>Участник.</returns>
    public static string FillMember(UserEntity user)
    {
        // Если у пользователя заполнены имя и фамилия, то запишем их.
        if (!string.IsNullOrEmpty(user.FirstName)
            && !string.IsNullOrEmpty(user.LastName))
        {
            return user.FirstName + " " + user.LastName;
        }

        // Если логин заполнен, то запишем его.
        if (!string.IsNullOrEmpty(user.Login))
        {
            return user.Login;
        }

        // Иначе запишем Email.
        return user.Email;
    }
}