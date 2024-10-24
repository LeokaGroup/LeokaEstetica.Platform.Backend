using LeokaEstetica.Platform.Classifiers.Abstractions.Communications;
using LeokaEstetica.Platform.Models.Dto.Base.Communications;
using LeokaEstetica.Platform.Models.Enums;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Classifiers.Services.Communications;

/// <summary>
/// Класс реализует методы сервиса классификации групп абстрактных областей чата.
/// </summary>
internal class ClassifierAbstractGroupService : IClassifierAbstractGroupService
{
    private readonly ILogger<ClassifierAbstractGroupService> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    public ClassifierAbstractGroupService(ILogger<ClassifierAbstractGroupService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<ClassifierResult> RunClassificationAsync(AbstractScopeTypeEnum abstractScopeType,
        DialogGroupTypeEnum groupType)
    {
        try
        {
            var result = new ClassifierResult();

            // Если абстрактная область - компания и если тип группировки диалогов проектов компании,
            // то группа объектов это проекты. 
            if (abstractScopeType == AbstractScopeTypeEnum.Company 
                && groupType == DialogGroupTypeEnum.Project)
            {
                result.ClassifierResultType = ClassifierResultTypeEnum.Project;
            }

            // Если абстрактная область - компания и если тип группировки диалогов компании,
            // то группа объектов это диалоги компании.
            else if (abstractScopeType == AbstractScopeTypeEnum.Company
                     && groupType == DialogGroupTypeEnum.Company)
            {
                result.ClassifierResultType = ClassifierResultTypeEnum.Company;
            }

            return Task.FromResult(result);
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}