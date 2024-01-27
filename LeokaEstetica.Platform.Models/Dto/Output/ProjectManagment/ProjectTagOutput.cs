using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Models.Extensions;
using Enum = LeokaEstetica.Platform.Models.Enums.Enum;

namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс выходной модели тегов проекта.
/// </summary>
public class ProjectTagOutput
{
    /// <summary>
    /// Id тега.
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
    /// Статус полёта
    /// </summary>
    public IEnum ObjectTagType
    {
        get => new Enum(Enum.ObjectTagType)
        {
            Value = ObjectTagType.ToString().ToSnakeCase()
        };

        set => ObjectTagTypeValue = System.Enum.Parse<ObjectTagTypeEnum>(value.Value.ToPascalCase());
    }
}