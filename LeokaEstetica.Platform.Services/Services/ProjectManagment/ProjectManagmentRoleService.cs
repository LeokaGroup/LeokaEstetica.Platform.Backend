using System.Runtime.CompilerServices;
using System.Text;
using AutoMapper;
using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Common.Cache;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Redis.Abstractions.ProjectManagement;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Services.Services.ProjectManagment;

/// <summary>
/// Класс реализует методы сервиса ролей управления проектами.
/// </summary>
internal sealed class ProjectManagmentRoleService : IProjectManagmentRoleService
{
    private readonly ILogger<ProjectManagmentRoleService>? _logger;
    private readonly Lazy<IProjectManagmentRoleRepository> _projectManagmentRoleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProjectManagmentRoleRedisService _projectManagmentRoleRedisService;
    private readonly IMapper _mapper;
    private readonly Lazy<IDiscordService> _discordService;
    private readonly Lazy<IHubNotificationService> _hubNotificationService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="projectManagmentRoleRepository">Репозиторий ролей.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="projectManagmentRoleRedisService">Сервис ролей кэша.</param>
    /// <param name="mapper">Маппер.</param>
    /// <param name="discordService">Сервис уведомлений дискорда.</param>
    /// <param name="discordService">Сервис уведомлений хабов.</param>
    public ProjectManagmentRoleService(ILogger<ProjectManagmentRoleService>? logger,
        Lazy<IProjectManagmentRoleRepository> projectManagmentRoleRepository,
        IUserRepository userRepository,
        IProjectManagmentRoleRedisService projectManagmentRoleRedisService,
        IMapper mapper,
        Lazy<IDiscordService> discordService,
         Lazy<IHubNotificationService> hubNotificationService)
    {
        _logger = logger;
        _projectManagmentRoleRepository = projectManagmentRoleRepository;
        _userRepository = userRepository;
        _projectManagmentRoleRedisService = projectManagmentRoleRedisService;
        _mapper = mapper;
        _discordService = discordService;
        _hubNotificationService = hubNotificationService;
    }

    #region Публичные методы.

    /// <inheritdoc />
    public async Task<IEnumerable<ProjectManagementRoleOutput>> GetUserRolesAsync(string account,
        long projectId, long companyId)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0) 
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            _logger?.LogInformation($"Проверяем роли пользователя: {userId} в кэше Redis...");

            // Проверяем, есть ли настройки в кэше.
            var cacheRoles = (await _projectManagmentRoleRedisService.GetUserRolesAsync(userId))?.AsList();

            // В кэше нашли роли пользователя - отдаем.
            if (cacheRoles is not null && cacheRoles.Count > 0)
            {
                _logger?.LogInformation($"Нашли роли пользователя: {userId} в кэше Redis...");
                
                var result = _mapper.Map<IEnumerable<ProjectManagementRoleOutput>>(cacheRoles);

                return result;
            }

            _logger?.LogInformation($"Не нашли роли пользователя: {userId} в кэше Redis...");
            _logger?.LogInformation($"Проверяем роли пользователя: {userId} в БД...");

            var userRoles = (await _projectManagmentRoleRepository.Value.GetUserRolesAsync(userId, projectId, companyId))
                ?.AsList();

            // Нашли в БД - отдаем.
            if (userRoles is not null && userRoles.Count > 0)
            {
                _logger?.LogInformation($"Нашли роли пользователя: {userId} в БД...");
                
                // Добавляем роли пользователя в кэш - чтобы в след.раз уже забрать оттуда.
                var mapRedisRoles = _mapper.Map<IEnumerable<ProjectManagementRoleRedis>>(userRoles);
                await _projectManagmentRoleRedisService.SetUserRolesAsync(userId, mapRedisRoles);
                
                return userRoles;
            }

            var exBuilder = new InvalidOperationException("Не удалось определить роли пользователя для модуля УП. " +
                                                          $"UserId: {userId}. " +
                                                          $"ProjectId: {projectId}. " +
                                                          $"CompanyId: {companyId}.");

            _logger?.LogInformation($"Не нашли роли пользователя: {userId} ни в кэше Redis, ни в БД...");

            throw new InvalidOperationException(exBuilder.ToString());
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateRolesAsync(IEnumerable<ProjectManagementRoleInput> roles, string account)
    {
        try
        {
            // Обновляем роли в БД.
            var projectManagementRoles = roles.AsList();
            await _projectManagmentRoleRepository.Value.UpdateRolesAsync(projectManagementRoles);

            // TODO: Также нужно обновлять роли в БД, а не ио
            // Обновляем роли в кэше.
            foreach (var r in projectManagementRoles)
            {
                var cacheRoles = (await _projectManagmentRoleRedisService.GetUserRolesAsync(r.UserId))?.AsList();
                
                if (cacheRoles is not null && cacheRoles.Count > 0)
                {
                    // Актуализируем активность ролей.
                    foreach (var cr in cacheRoles)
                    {
                        cr.IsEnabled = r.IsEnabled;
                    }
                    
                    await _projectManagmentRoleRedisService.SetUserRolesAsync(r.UserId, cacheRoles);
                }

                // Иначе ищем в БД роли.
                else
                {
                    var userRoles = (await _projectManagmentRoleRepository.Value.GetUserRolesAsync(r.UserId, null,
                        null))?.AsList();

                    if (userRoles is null || userRoles.Count <= 0)
                    {
                        var ex = new InvalidOperationException(
                            "Не удалось получить роли из БД. " +
                            $"UserId: {r.UserId}. " +
                            $"Список ролей был к обновлению: {JsonConvert.SerializeObject(roles)}");
                                                               
                        await _discordService.Value.SendNotificationErrorAsync(ex);
                        _logger?.LogError(ex, ex.Message);
                        
                        // Не ломаем приложение, но логируем такое.
                        continue;
                    }
                    
                    var mapRedisRoles = _mapper.Map<IEnumerable<ProjectManagementRoleRedis>>(userRoles);
                    await _projectManagmentRoleRedisService.SetUserRolesAsync(r.UserId, mapRedisRoles);
                }
            }
            
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);

            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
                "Роли успешно сохранены.", NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS,
                "SendNotifySuccessUpdateRoles", userCode, UserConnectionModuleEnum.ProjectManagement);
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    #endregion

    #region Приватные методы.

    #endregion
}