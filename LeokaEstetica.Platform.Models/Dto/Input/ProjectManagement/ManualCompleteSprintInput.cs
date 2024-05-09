using LeokaEstetica.Platform.Models.Dto.Base.ProjectManagement;

namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели завершения спринта.
/// </summary>
public class ManualCompleteSprintInput : SprintInput
{
    /// <summary>
    /// Данные действий пользователя, если они есть.
    /// </summary>
    public BaseNeedSprintAction? NeedSprintAction { get; set; }
}