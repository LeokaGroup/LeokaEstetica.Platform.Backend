using LeokaEstetica.Platform.Models.Dto.Base.Communications;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Classifiers.Abstractions.Communications;

/// <summary>
/// Абстракция сервиса классификации групп абстрактных областей чата.
/// </summary>
public interface IClassifierAbstractGroupService
{
    /// <summary>
    /// Метод запускает классификацию абстрактной области, чтобы определить ее группы.
    /// Результатом является группа, объекты которой нужно будет получить.
    /// </summary>
    /// <param name="abstractScopeType">Тип абстрактной области.</param>
    /// <returns>Группа, объекты которой нужно будет получить.</returns>
    Task<ClassifierResult> RunClassificationAsync(AbstractScopeTypeEnum abstractScopeType);
}