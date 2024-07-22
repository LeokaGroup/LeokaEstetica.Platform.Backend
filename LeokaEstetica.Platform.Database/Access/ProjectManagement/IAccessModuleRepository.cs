using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Database.Access.ProjectManagement;

/// <summary>
/// Абстракция репозитория проверки доступа к разным модулям платформы.
/// </summary>
public interface IAccessModuleRepository
{
    /// <summary>
    /// Метод проверяет доступ к модулям, компонентам модуля УП.
    /// Бесплатные компоненты не проверяются.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="accessModule">Тип модуля.</param>
    /// <param name="accessModuleComponentType">Тип компонента, к которому проверяется доступ.</param>
    /// <returns>Модель с результатами проверки доступа.</returns>
    Task<bool> CheckAccessProjectManagementModuleOrComponentAsync(long projectId, AccessModuleTypeEnum accessModule,
        AccessModuleComponentTypeEnum accessModuleComponentType);
}