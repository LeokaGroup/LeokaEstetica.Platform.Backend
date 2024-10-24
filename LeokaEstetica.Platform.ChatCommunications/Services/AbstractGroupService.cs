﻿using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Classifiers.Abstractions.Communications;
using LeokaEstetica.Platform.Communications.Abstractions;
using LeokaEstetica.Platform.Database.Abstractions.Communications;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Communications.Output;
using LeokaEstetica.Platform.Models.Enums;
using Enum = System.Enum;

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
    private readonly IAbstractGroupObjectsRepository _abstractGroupObjectsRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="classifierAbstractGroupService">Сервис классикатора групп областей чата.</param>
    /// <param name="logger">Логгер.</param>
    /// <param name="companyRepository">Репозиторий компаний.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="abstractGroupObjectsRepository">Репозиторий объектов.</param>
    public AbstractGroupService(IClassifierAbstractGroupService classifierAbstractGroupService,
        ILogger<AbstractGroupService> logger,
        Lazy<ICompanyRepository> companyRepository,
        IUserRepository userRepository,
        IAbstractGroupObjectsRepository abstractGroupObjectsRepository)
    {
        _classifierAbstractGroupService = classifierAbstractGroupService;
        _logger = logger;
        _companyRepository = companyRepository;
        _userRepository = userRepository;
        _abstractGroupObjectsRepository = abstractGroupObjectsRepository;
    }

    /// <inheritdoc />
    public async Task<AbstractGroupResult> GetAbstractGroupObjectsAsync(long abstractScopeId,
        AbstractScopeTypeEnum abstractScopeType, string account, DialogGroupTypeEnum groupType)
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
            var group = await _classifierAbstractGroupService.RunClassificationAsync(abstractScopeType, groupType);

            // Если получаем диалоги проектов компании.
            // То получаем проекты компании, содержащие свои диалоги.
            if (group.ClassifierResultType == ClassifierResultTypeEnum.Project)
            {
                // Получаем проекты компании и где текущий пользователь есть в участниках.
                result.GroupName = "Проекты компании";
                result.GroupSysName = "CompanyProjects";
                result.Objects = (await _companyRepository.Value.GetCompanyProjectsAsync(abstractScopeId, userId))
                    ?.AsList();
                result.DialogGroupType = DialogGroupTypeEnum.Project.ToString();
            }

            // Если получаем диалоги компании.
            if (group.ClassifierResultType == ClassifierResultTypeEnum.Company)
            {
                result.GroupName = "Диалоги компании";
                result.GroupSysName = "CompanyDialogs";

                result.Objects ??= new List<AbstractGroupOutput>();

                var companyDialogs = (await _abstractGroupObjectsRepository.GetCompanyDialogsAsync(
                    abstractScopeId))?.AsList();

                // Заполняем лишь первый элемент, т.к. на фронте выведем все диалоги просто первого объекта группы.
                // Нету такого как с проектами, внутри которых диалоги, тут сразу диалоги получить можем.
                result.Objects.Add(new AbstractGroupOutput
                {
                    AbstractGroupId = abstractScopeId,
                    UserId = userId,
                    AbstractGroupType = AbstractGroupTypeEnum.Project,
                    HasDialogs = companyDialogs?.Count > 0,
                    Items = companyDialogs,
                });
                
                result.DialogGroupType = DialogGroupTypeEnum.Company.ToString();
            }
            
            // Отбираем объекты, у которых есть диалоги с сообщениями и заполняем объекты сообщениями.
            if (result.Objects is not null && result.Objects.Count > 0)
            {
                // Получаем диалоги каждого объекта.
                var objectIds = result.Objects.Select(x => x.AbstractGroupId);
                var objectDialogs = (await _abstractGroupObjectsRepository.GetObjectDialogsAsync(objectIds))
                    ?.AsList();
                
                // Заполняем объекты.
                foreach (var o in result.Objects)
                {
                    o.Items ??= new List<GroupObjectDialogOutput>();

                    if (string.IsNullOrWhiteSpace(result.DialogGroupType))
                    {
                        throw new InvalidOperationException(
                            "Тип группировки не был задан. К этому моменту он уже должен быть известен.");
                    }

                    // Заполняем объект диалогами.
                    if (o.HasDialogs
                        && objectDialogs?.Count > 0
                        && Enum.Parse<DialogGroupTypeEnum>(result.DialogGroupType) != DialogGroupTypeEnum.Company)
                    {
                        o.Items.AddRange(objectDialogs.Where(x => x.ObjectId == o.AbstractGroupId));
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