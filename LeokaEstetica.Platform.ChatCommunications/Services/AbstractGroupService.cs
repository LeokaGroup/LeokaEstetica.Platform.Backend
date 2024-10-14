using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Classifiers.Abstractions.Communications;
using LeokaEstetica.Platform.Communications.Abstractions;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Communications.Output;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Communications.Services;

/// <summary>
/// Класс реализует методы сервиса групп абстрактной области чата.
/// </summary>
internal sealed class AbstractGroupService : IAbstractGroupService
{
    private readonly IClassifierAbstractGroupService _classifierAbstractGroupService;
    private readonly ILogger<AbstractGroupService> _logger;
    private readonly Lazy<ICompanyRepository> _companyRepository;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="classifierAbstractGroupService">Сервис классикатора групп областей чата.</param>
    /// <param name="logger">Логгер.</param>
    /// <param name="companyRepository">Репозиторий компаний.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    public AbstractGroupService(IClassifierAbstractGroupService classifierAbstractGroupService,
        ILogger<AbstractGroupService> logger,
        Lazy<ICompanyRepository> companyRepository,
        IUserRepository userRepository)
    {
        _classifierAbstractGroupService = classifierAbstractGroupService;
        _logger = logger;
        _companyRepository = companyRepository;
        _userRepository = userRepository;
    }

    /// <inheritdoc />
    public async Task<AbstractGroupResult> GetAbstractGroupObjectsAsync(long abstractScopeId,
        AbstractScopeTypeEnum abstractScopeType, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId == 0)
            {
                throw new InvalidOperationException($"Id пользователя с аккаунтом {account} не найден.");
            }
            
            var result = new AbstractGroupResult();
            
            // Классификатор определяет группу, по которой поймем, объекты какой группы нужно получать.
            var group = await _classifierAbstractGroupService.RunClassificationAsync(abstractScopeType);

            if (group.ClassifierResultType == ClassifierResultTypeEnum.Project)
            {
                // Получаем проекты компании и где текущий пользователь есть в участниках.
                result.GroupName = "Проекты компании";
                result.Objects = (await _companyRepository.Value.GetAbstractGroupObjectsAsync(abstractScopeId, userId))
                    ?.AsList();
            }
            
            // Отбираем объекты, у которых есть диалоги с сообщениями и заполняем объекты сообщениями.
            if (result.Objects is not null && result.Objects.Count > 0)
            {
                foreach (var o in result.Objects)
                {
                    // Вместо null будет пустой массив, фронту так проще будет.
                    o.Items ??= new List<EmptyObjectItem>();

                    if (o.HasDialogs)
                    {
                        // Просто добавляем пустышку, чтобы фронт мог отразить наличие вложенности.
                        // Уже будет > 1 и фронт отразит стрелочку у вложенности.
                        // Т.к. на фронте контрол смотрит именно на кол-во items.
                        o.Items.Add(new EmptyObjectItem());   
                    }
                }
            }

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}