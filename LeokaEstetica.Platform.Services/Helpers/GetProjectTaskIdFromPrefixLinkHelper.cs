namespace LeokaEstetica.Platform.Services.Helpers;

/// <summary>
/// Класс хелпера для получения из префиксной строки Id задачи в рамках проекта.
/// </summary>
public static class GetProjectTaskIdFromPrefixLinkHelper
{
    /// <summary>
    /// Метод получает Id задачи в рамках проекта из префиксной строки.
    /// </summary>
    /// <param name="projectTaskId">Префиксная строка содержащая префикс + Id задачи в рамках проекта.</param>
    /// <returns>Id задачи в рамках проекта.</returns>
    public static long GetProjectTaskIdFromPrefixLink(this string projectTaskId)
    {
        return Convert.ToInt64(projectTaskId.Split("-").LastOrDefault() ?? throw new InvalidOperationException(
            $"Не удалось извлечь Id задачи из префиксной строки {projectTaskId}."));
    }
}