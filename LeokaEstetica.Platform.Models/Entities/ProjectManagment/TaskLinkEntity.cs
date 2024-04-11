using System.ComponentModel.DataAnnotations.Schema;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Models.Extensions;

namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// Класс сопоставляется с таблицей связей задач.
/// </summary>
public class TaskLinkEntity
{
    /// <summary>
    /// PK. Id связи.
    /// </summary>
    public long LinkId { get; set; }

    /// <summary>
    /// Id задачи, которую связывают.
    /// </summary>
    public long? FromTaskId { get; set; }
    
    /// <summary>
    /// Id задачи, с которой связывают.
    /// </summary>
    public long? ToTaskId { get; set; }

    /// <summary>
    /// Значение типа связи.
    /// </summary>
    public LinkTypeEnum LinkTypeValue { get; set; }
    
    /// <summary>
    /// Тип связи.
    /// </summary>
    [NotMapped]
    public IEnum LinkTypeEnumType
    {
        get => new Enums.Enum(Enums.Enum.LinkType)
        {
            Value = LinkTypeValue.ToString().ToSnakeCase()
        };

        set => LinkTypeValue = System.Enum.Parse<LinkTypeEnum>(value.Value.ToPascalCase());
    }

    /// <summary>
    /// Id родительской задачи.
    /// </summary>
    public long? ParentId { get; set; }

    /// <summary>
    /// Id дочерней задачи.
    /// </summary>
    public long? ChildId { get; set; }

    /// <summary>
    /// Признак блокирующей задачи.
    /// </summary>
    public bool IsBlocked { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Id блокируемой задачи.
    /// Задача, которая ее блокирует определяется по признаку IsBlocked.
    /// Блокируемая задача становится зависимой от блокирующей.
    /// </summary>
    public long? BlockedTaskId { get; set; }
}