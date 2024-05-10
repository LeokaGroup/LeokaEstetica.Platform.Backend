using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Models.Dto.Base.ProjectManagement;

/// <summary>
/// Класс выходной модели, если для действия со спринтами понадобились какие-то действия от пользователя.
/// </summary>
public class BaseNeedSprintAction
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="needSprintActionType">Тип действия, которое ожидается от пользователя.</param>
    public BaseNeedSprintAction(NeedSprintActionTypeEnum needSprintActionType)
    {
        NeedSprintActionType = needSprintActionType;

        // Если действия с незавершенными задачами спринта.
        if (needSprintActionType == NeedSprintActionTypeEnum.NotCompletedTask)
        {
            ActionVariants = new List<NeedSprintActionVariants>
            {
                new ()
                {
                    VariantName = "В бэклог",
                    VariantSysName = "Backlog",
                    IsAvailable = true
                },
                
                new ()
                {
                    VariantName = "В один из будущих спринтов",
                    VariantSysName = "Prospective",
                    NotAvailableTooltip = "Действие недоступно - нет спланированных спринтов.",
                    IsAvailable = true
                },
                
                new ()
                {
                    VariantName = "В новый спринт",
                    VariantSysName = "NewSprint",
                    IsAvailable = true
                }
            };
        }
    }

    /// <summary>
    /// Признак необходимости какого-либо действия от пользователя.
    /// </summary>
    public bool IsNeedUserAction { get; set; }

    /// <summary>
    /// Тип действия, которое ожидается от пользователя.
    /// </summary>
    public NeedSprintActionTypeEnum NeedSprintActionType { get; set; }

    /// <summary>
    /// Список действий.
    /// </summary>
    public List<NeedSprintActionVariants>? ActionVariants { get; set; }
}

/// <summary>
/// Класс вариантов действий.
/// </summary>
public class NeedSprintActionVariants
{
    /// <summary>
    /// Название варианта.
    /// </summary>
    public string? VariantName { get; set; }
    
    /// <summary>
    /// Системное название варианта.
    /// </summary>
    public string? VariantSysName { get; set; }

    /// <summary>
    /// Признак выбора действия пользователем.
    /// </summary>
    public bool IsSelected { get; set; }

    /// <summary>
    /// Признак активности варианта.
    /// Если проходим по условиям, то пункт активен.
    /// </summary>
    public bool IsAvailable { get; set; }

    /// <summary>
    /// Подсказка причины неактивности варианта действия.
    /// </summary>
    public string? NotAvailableTooltip { get; set; }
}