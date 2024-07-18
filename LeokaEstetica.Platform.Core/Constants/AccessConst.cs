namespace LeokaEstetica.Platform.Core.Constants;

/// <summary>
/// Класс констант для проверки доступов.
/// </summary>
public static class AccessConst
{
    /// <summary>
    /// Если не передали Id проекта.
    /// </summary>
    public const string NOT_VALID_PROJECT_ID = "Id проекта не передан для проверки доступа.";
    
    /// <summary>
    /// Если не передали тип модуля.
    /// </summary>
    public const string NOT_VALID_MODULE_TYPE = "Тип модуля не передан для проверки доступа.";
    
    /// <summary>
    /// Если не передали тип компонента модуля.
    /// </summary>
    public const string NOT_VALID_MODULE_COMPONENT_TYPE = "Тип компонента модуля не передан для проверки доступа.";
}