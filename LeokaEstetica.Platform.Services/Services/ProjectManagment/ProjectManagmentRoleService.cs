﻿using System.Runtime.CompilerServices;
using System.Text;
using AutoMapper;
using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Models.Dto.Common.Cache;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement;
using LeokaEstetica.Platform.Redis.Abstractions.ProjectManagement;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using Microsoft.Extensions.Logging;

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

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="projectManagmentRoleRepository">Репозиторий ролей.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="projectManagmentRoleRedisService">Сервис ролей кэша.</param>
    /// <param name="mapper">Маппер.</param>
    public ProjectManagmentRoleService(ILogger<ProjectManagmentRoleService>? logger,
        Lazy<IProjectManagmentRoleRepository> projectManagmentRoleRepository,
        IUserRepository userRepository,
        IProjectManagmentRoleRedisService projectManagmentRoleRedisService,
         IMapper mapper)
    {
        _logger = logger;
        _projectManagmentRoleRepository = projectManagmentRoleRepository;
        _userRepository = userRepository;
        _projectManagmentRoleRedisService = projectManagmentRoleRedisService;
        _mapper = mapper;
    }

    #region Публичные методы.

    /// <inheritdoc />
    public async Task<IEnumerable<ProjectManagementRoleOutput>> GetUserRolesAsync(string account,
        long? projectId = null)
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

            var userRoles = (await _projectManagmentRoleRepository.Value.GetUserRolesAsync(userId, projectId))
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

            var exBuilder = new StringBuilder("Не удалось определить роли пользователя для модуля УП. " +
                                              $"UserId: {userId}.");

            if (projectId.HasValue)
            {
                exBuilder.AppendLine($"ProjectId: {projectId}");
            }

            _logger?.LogInformation($"Не нашли роли пользователя: {userId} ни в кэше Redis, ни в БД...");

            throw new InvalidOperationException(exBuilder.ToString());
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