using LeokaEstetica.Platform.Models.Extensions;

namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Класс для маппинга типов SQL Enums в Postgres.
/// </summary>
public class Enum : IEnum
{
    /// <summary>
    /// Тип Enum для типов тегов.
    /// </summary>
    public const string ObjectTagType = "object_tag_type_enum";
    
    /// <summary>
    /// Тип Enum для типов связей задач.
    /// </summary>
    public const string LinkType = "link_type_enum";

    /// <summary>
    /// Тип Enum для типа документа.
    /// </summary>
    public const string DocumentType = "document_type_enum";

    /// <summary>
    /// Тип Enum для типа перехода.
    /// </summary>
    public const string TransitionType = "transition_type_enum";
    
    /// <summary>
    /// Тип Enum для типа заадчи.
    /// </summary>
    public const string TaskType = "task_type_enum";
    
    /// <summary>
    /// Тип Enum для типа стратегии пользователя.
    /// </summary>
    public const string ProjectStrategy = "project_strategy_enum";

    /// <summary>
    /// Тип Enum для типа диалога нейросети.
    /// </summary>
    public const string ObjectTypeDialogAi = "object_type_dialog_ai";

    /// <summary>
    /// Тип Enum для типа модуля.
    /// </summary>
    public const string AccessModuleType = "access_module_type_enum";
    
    /// <summary>
    /// Тип Enum для типа компонента модуля.
    /// </summary>
    public const string AccessModuleComponentType = "access_module_component_type_enum";
    
    /// <summary>
    /// Тип Enum для типа подписки.
    /// </summary>
    public const string SubscriptionType = "subscription_type_enum";

    /// <summary>
    /// Тип Enum для компонентной роли.
    /// </summary>
    public const string ComponentRole = "component_role_enum";

    /// <summary>
    /// Тип Enum для типа валюты.
    /// </summary>
    public const string CurrencyType = "currency_enum";
    
    /// <summary>
    /// Тип Enum для типа заказа.
    /// </summary>
    public const string OrderType = "order_type_enum";

    /// <summary>
    /// Конструктор по дефолту.
    /// </summary>
    /// <param name="type">Тип Enum в базе данных.</param>
    public Enum(string type = null)
    {
        Type = type;
    }

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="value">Значение типа тега.</param>
    public Enum(ObjectTagTypeEnum value)
    {
        Type = ObjectTagType;
        Value = value.ToString().ToSnakeCase();
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="value">Значение типа связи задачи.</param>
    public Enum(LinkTypeEnum value)
    {
        Type = LinkType;
        Value = value.ToString().ToSnakeCase();
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="value">Значение типа документа.</param>
    public Enum(DocumentTypeEnum value)
    {
        Type = DocumentType;
        Value = value.ToString().ToSnakeCase();
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="value">Значение типа перехода.</param>
    public Enum(TransitionTypeEnum value)
    {
        Type = TransitionType;
        Value = value.ToString().ToSnakeCase();
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="value">Значение типа задачи.</param>
    public Enum(SearchAgileObjectTypeEnum value)
    {
        Type = TaskType;
        Value = value.ToString().ToSnakeCase();
    }

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="value">Значение типа стратегии.</param>
    public Enum(ProjectStrategyEnum value)
    {
        Type = ProjectStrategy;
        Value = value.ToString().ToSnakeCase();
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="value">Значение типа диалога.</param>
    public Enum(DiscussionTypeEnum value)
    {
        Type = ObjectTypeDialogAi;
        Value = value.ToString().ToSnakeCase();
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="value">Значение типа.</param>
    public Enum(AccessModuleTypeEnum value)
    {
        Type = AccessModuleType;
        Value = value.ToString();
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="value">Значение типа.</param>
    public Enum(AccessModuleComponentTypeEnum value)
    {
        Type = AccessModuleComponentType;
        Value = value.ToString();
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="value">Значение типа.</param>
    public Enum(SubscriptionTypeEnum value)
    {
        Type = SubscriptionType;
        Value = value.ToString();
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="value">Значение типа.</param>
    public Enum(ComponentRoleEnum value)
    {
        Type = ComponentRole;
        Value = value.ToString().ToSnakeCase();
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="value">Значение типа.</param>
    public Enum(CurrencyTypeEnum value)
    {
        Type = CurrencyType;
        Value = value.ToString();
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="value">Значение типа.</param>
    public Enum(OrderTypeEnum value)
    {
        Type = OrderType;
        Value = value.ToString();
    }

    /// <inheritdoc/>
    public string Value { get; set; }

    /// <inheritdoc/>
    public string Type { get; }
    
    /// <summary>
    /// Получает новое значение типа TEnum из строки.
    /// </summary>
    /// <typeparam name="TEnum"> Тип данных. </typeparam>
    /// <param name="stringValue"> Строковое предстваление данных. </param>
    /// <returns> Значение TEnum. </returns>
    public static TEnum FromString<TEnum>(string stringValue)
        where TEnum : struct
    {
        return System.Enum.Parse<TEnum>(stringValue.ToPascalCase());
    }
}