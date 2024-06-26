namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement.Output;

/// <summary>
/// Класс выходной модели удаления папки дерева wiki проекта.
/// </summary>
public class RemoveFolderResponseOutput
{
    /// <summary>
    /// Признак ожидания действия от пользователя.
    /// </summary>
    public bool IsNeedUserAction { get; set; }

    /// <summary>
    /// Текст ответа, если удаление не произошло и требуются действия от пользователя.
    /// </summary>
    public string? ResponseText { get; set; }
}