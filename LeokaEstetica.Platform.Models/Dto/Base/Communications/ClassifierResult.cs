using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Models.Dto.Base.Communications;

/// <summary>
/// Класс результата работы классификатора.
/// </summary>
public class ClassifierResult
{
    /// <summary>
    /// Тип группы абстрактной области чата, которую определил классификатор.
    /// </summary>
    public ClassifierResultTypeEnum ClassifierResultType { get; set; }
}