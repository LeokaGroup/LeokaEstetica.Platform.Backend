using LeokaEstetica.Platform.Access.Models.Output;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Access.Abstractions.ProjectManagement;

/// <summary>
/// Абстракция сервиса проверки доступа к разным модулям платформы.
/// </summary>
public interface IAccessModuleService
{
    /// <summary>
    /// Метод проверяет доступ к модулям, компонентам модуля УП.
    /// Бесплатные компоненты не проверяются.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="accessModule">Тип модуля.</param>
    /// <param name="accessModuleComponentType">Тип компонента, к которому проверяется доступ.</param>
    /// <returns>Модель с результатами проверки доступа.</returns>
    Task<AccessProjectManagementOutput> CheckAccessProjectManagementModuleOrComponentAsync(long projectId,
        AccessModuleTypeEnum accessModule, AccessModuleComponentTypeEnum accessModuleComponentType);
}