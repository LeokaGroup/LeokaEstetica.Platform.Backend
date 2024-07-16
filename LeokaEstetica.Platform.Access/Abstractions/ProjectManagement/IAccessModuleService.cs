using LeokaEstetica.Platform.Access.Models.Output;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Access.Abstractions.ProjectManagement;

/// <summary>
/// Абстракция проверки доступа к разным модулям платформы.
/// </summary>
public interface IAccessModuleService
{
    Task<AccessProjectManagementOutput> CheckAccessProjectManagementModuleOrComponentAsync(long projectId,
        AccessModuleTypeEnum accessModule);
}