namespace LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;

/// <summary>
/// Абстракция репозитория Scrum Master AI.
/// </summary>
public interface IScrumMasterAiRepository
{
    /// <summary>
    /// Метод получает актуальную версию модели нейросети.
    /// </summary>
    /// <returns>Версия нейросети.</returns>
    Task<string?> GetLastNetworkVersionAsync();
}