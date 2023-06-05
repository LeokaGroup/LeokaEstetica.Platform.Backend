using LeokaEstetica.Platform.Models.Dto.Output.Metrics;

namespace LeokaEstetica.Platform.Diagnostics.Helpers;

/// <summary>
/// Класс хелпера метрик пользователей.
/// </summary>
public static class UserMetricsHelper
{
    private const string _helloText = "Поприветствуем нового пользователя ";

    /// <summary>
    /// Метод записывает текст приветствия новых пользователей.
    /// <param name="users">Список пользователей.</param>
    /// </summary>
    /// <returns>Список пользователей.</returns>
    public static IEnumerable<NewUserMetricsOutput> CreateDisplayTextNewUserMetrics(List<NewUserMetricsOutput> users)
    {
        foreach (var user in users)
        {
            // Проставляем логин, если он заполнен, иначе почту.
            var userText = !string.IsNullOrEmpty(user.Login) ? user.Login : user.Email;
            user.DisplayText = _helloText + userText;
        }

        return users;
    }
}