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
    public Task<ClassifierResult> RunClassificationAsync(AbstractScopeTypeEnum abstractScopeType)
    {
        try
        {
            var result = new ClassifierResult();

            // Если абстрактная область - компания, то группа объектов это проекты.
            // Для такой классификации достаточно знать, что пришел тип - компания.
            // В будущем возможно будут еще вакансии учитываться, а не только проекты.
            // Тогда классификация изменится и усложнится.
            if (abstractScopeType == AbstractScopeTypeEnum.Company)
            {
                result.ClassifierResultType = ClassifierResultTypeEnum.Project;
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