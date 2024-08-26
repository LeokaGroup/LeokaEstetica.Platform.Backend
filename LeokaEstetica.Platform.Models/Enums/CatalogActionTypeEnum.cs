namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление типов действия каталога.
/// </summary>
public enum CatalogActionTypeEnum
{
    /// <summary>
    /// Неизвестный тип.
    /// </summary>
    Undefined = 0,
    
    /// <summary>
    /// Получение первой и последующих страниц каталога.
    /// </summary>
    Catalog = 1,
    
    /// <summary>
    /// Применение фильтров каталога.
    /// </summary>
    Filter = 2,
    
    /// <summary>
    /// Поиск в каталоге.
    /// </summary>
    Search = 3
}