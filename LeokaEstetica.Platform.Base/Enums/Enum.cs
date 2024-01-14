namespace LeokaEstetica.Platform.Base.Enums;

/// <summary>
/// Класс для преставления типа SQL Enums в Postgres.
/// </summary>
internal class Enum : IEnum
{
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="Enum"/>.
    /// </summary>
    /// <param name="type"> Тип Enum в базе данных. </param>
    public Enum(string type = null)
    {
        Type = type;
    }
    
    /// TODO: Оставил пока для примера как мапить на енамку. Убрать когда появится первый тип.
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="Enum"/>
    /// </summary>
    /// <param name="value"> Тип документа. </param>
    // public Enum(DocumentTypes value)
    // {
    //     Type = DocumentsTypes;
    //     Value = value.ToString().ToSnakeCase();
    // }

    /// TODO: Оставил пока для примера как мапить на енамку. Убрать когда появится первый тип.
    /// <summary>
    /// Тип Enum для типов документов
    /// </summary>
    //public const string DocumentsTypes = "enum_document_types";
    
    /// <inheritdoc/>
    public string Value { get; set; }
    
    /// <inheritdoc/>
    public string Type { get; }
}