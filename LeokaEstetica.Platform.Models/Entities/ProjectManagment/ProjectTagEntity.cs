using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Models.Extensions;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Models.Entities.ProjectManagment")]

namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// Класс сопоставляется с таблицей тегов (меток) проекта.
/// </summary>
public class ProjectTagEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public int TagId { get; set; }

    /// <summary>
    /// Название тега.
    /// </summary>
    public string TagName { get; set; }

    /// <summary>
    /// Системное название тега.
    /// </summary>
    public string TagSysName { get; set; }
    
    /// <summary>
    /// Порядковый номер.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Описание метки (тега).
    /// </summary>
    public string TagDescription { get; set; }
    
    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Тип объекта тега.
    /// </summary>
    public ObjectTagTypeEnum ObjectTagTypeValue { get; set; }
    
    /// <summary>
    /// Тип тега.
    /// </summary>
    [NotMapped]
    public IEnum ObjectTagType
    {
        get => new Enums.Enum(Enums.Enum.ObjectTagType)
        {
            Value = ObjectTagTypeValue.ToString().ToSnakeCase()
        };

        set => ObjectTagTypeValue = System.Enum.Parse<ObjectTagTypeEnum>(value.Value.ToPascalCase());
    }
}