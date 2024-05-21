namespace LeokaEstetica.Platform.ProjectManagement.ScrumMasterAI.Abstractions;

/// <summary>
/// Абстракция сервиса нейросети Scrum Master AI.
/// </summary>
public interface IScrumMasterAiService
{
    /// <summary>
    /// TODO: Добавить ограничение на роль пользователя. Только некоторым можно учить ее, проверять на роль.
    /// Метод обучает нейросеть из датасета в формате csv.
    /// </summary>
    /// <param name="file">Датасет в csv.</param>
    Task EducationFromCsvDatasetAsync();
}