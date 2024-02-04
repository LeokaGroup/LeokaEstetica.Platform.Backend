using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс доступных к созданию связи задач.
/// </summary>
public class AvailableTaskLinkOutput : GetTaskLinkOutput
{
    /// <summary>
    /// Тип связи.
    /// </summary>
    public LinkTypeEnum LinkType { get; set; }
}