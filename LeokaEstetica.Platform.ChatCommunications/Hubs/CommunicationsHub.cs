﻿using LeokaEstetica.Platform.Base.Extensions.StringExtensions;
using LeokaEstetica.Platform.Communications.Abstractions;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Common.Cache.Output;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Redis.Abstractions.Connection;
using Microsoft.AspNetCore.SignalR;
using Enum = System.Enum;

namespace LeokaEstetica.Platform.Communications.Hubs;

/// <summary>
/// Класс хаба модуля коммуникаций.
/// </summary>
internal sealed class CommunicationsHub : Hub
{
    private readonly IAbstractScopeService _abstractScopeService;
    private readonly ILogger<CommunicationsHub> _logger;
    private readonly IDiscordService _discordService;
    private readonly IConnectionService _connectionService;
    private readonly IAbstractGroupService _abstractGroupService;
    private readonly IAbstractGroupDialogMessagesService _abstractGroupDialogMessagesService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="abstractScopeService">Сервис абстрактных областей чата.</param>
    /// <param name="logger">Логгер.</param>
    /// <param name="discordService">Сервис уведомлений дискорда.</param>
    /// <param name="connectionService">Сервис подключений Redis.</param>
    /// <param name="abstractGroupService">Сервис групп абстрактной области.</param>
    /// <param name="abstractGroupDialogMessagesService">Сервис сообщений диалога.</param>
    public CommunicationsHub(IAbstractScopeService abstractScopeService,
        ILogger<CommunicationsHub> logger,
        IDiscordService discordService,
        IConnectionService connectionService,
        IAbstractGroupService abstractGroupService,
        IAbstractGroupDialogMessagesService abstractGroupDialogMessagesService)
    {
        _abstractScopeService = abstractScopeService;
        _logger = logger;
        _discordService = discordService;
        _connectionService = connectionService;
        _abstractGroupService = abstractGroupService;
        _abstractGroupDialogMessagesService = abstractGroupDialogMessagesService;
    }

    #region Публичные методы.

    /// <inheritdoc />
    public override async Task OnConnectedAsync()
    {
        try
        {
            var userCode = Context.GetHttpContext()?.Request.Query["userCode"].ToString();
            var module = Enum.Parse<UserConnectionModuleEnum>(
                Context.GetHttpContext()?.Request.Query["module"].ToString()!);

            if (string.IsNullOrEmpty(userCode))
            {
                throw new InvalidOperationException("Ошибка получения кода пользователя.");
            }
            
            await _connectionService.AddConnectionIdCacheAsync(userCode, Context.ConnectionId, module)
                .ConfigureAwait(false);
        }
        
        catch (Exception ex)
        {
            await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);
            
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
    
    /// <inheritdoc />
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Метод получает список абстрактных областей чата.
    /// Учитывается текущий пользователь.
    /// Текущий метод можно расширять новыми абстрактными областями.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <exception cref="InvalidOperationException">Если ошибка валидации.</exception>
    /// <returns>Возвращает через сокеты cписок абстрактных областей чата.</returns>
    public async Task GetScopesAsync(string account)
    {
        try
        {
            // Получаем список абстрактных областей чата.
            var result = await _abstractScopeService.GetAbstractScopesAsync(account);
            
            var connection = await GetConnectionCacheAsync();

            await Clients
                .Client(connection.ConnectionId)
                .SendAsync("getAbstractScopes", result)
                .ConfigureAwait(false);
        }
        
        catch (Exception ex)
        {
            await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);
            
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает группы объектов выбранной абстрактной области чата.
    /// Группой объектов может являться список проектов компании либо список диалогов компании.
    /// Список диалогов компании не предполагает вложенность и список диалогов открывается сразу,
    /// а вот список проектов предполагает еще вложенность, каждый диалог по выбору проекта уже получаем.
    /// </summary>
    /// <param name="abstractScopeId">Id выбранной абстрактной области чата.</param>
    /// <param name="abstractScopeType">Тип выбранной абстрактной области чата.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="dialogGroupType">Тип группировки диалогов.</param>
    /// <exception cref="InvalidOperationException">Если ошибка валидации.</exception>
    /// <returns>Возвращает через сокеты группы объектов выбранной абстрактной области чата.</returns>
    public async Task GetScopeGroupObjectsAsync(long abstractScopeId, string? abstractScopeType,
        string account, string? dialogGroupType)
    {
        try
        {
            if (abstractScopeId <= 0)
            {
                throw new InvalidOperationException("Id абстрактной области невалиден. " +
                                                    $"abstractScopeId: {abstractScopeId}.");
            }

            if (string.IsNullOrWhiteSpace(abstractScopeType))
            {
                throw new InvalidOperationException("Не передан тип абстрактной области чата. " +
                                                    $"AbstractScopeType: {abstractScopeType}.");
            }
            
            // С фронта приходит в нижнем регистре, поэтому приводим к нотации PascalCase.
            var scopeType = Enum.Parse<AbstractScopeTypeEnum>(abstractScopeType.ToPascalCase());

            if (scopeType == AbstractScopeTypeEnum.Undefined)
            {
                throw new InvalidOperationException("Тип абстрактной области невалиден. " +
                                                    $"AbstractScopeType: {abstractScopeType}.");
            }
            
            if (string.IsNullOrWhiteSpace(dialogGroupType))
            {
                throw new InvalidOperationException("Не передан тип группировки диалогов чата. " +
                                                    $"DialogGroupType: {dialogGroupType}.");
            }

            var groupType = Enum.Parse<DialogGroupTypeEnum>(dialogGroupType.ToPascalCase());
            
            if (groupType == DialogGroupTypeEnum.Undefined)
            {
                throw new InvalidOperationException("Тип группировки диалогов чата невалиден. " +
                                                    $"DialogGroupType: {groupType}.");
            }

            // Получаем список групп объектов выбранной абстрактной области чата.
            var result = await _abstractGroupService.GetAbstractGroupObjectsAsync(abstractScopeId, scopeType,
                account, groupType);
                
            var connection = await GetConnectionCacheAsync();

            await Clients
                .Client(connection.ConnectionId)
                .SendAsync("getScopeGroupObjects", result)
                .ConfigureAwait(false);
        }
        
        catch (Exception ex)
        {
            await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);
            
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает сообщения диалога.
    /// </summary>
    /// <param name="dialogId">Id диалога.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Возвращает через сокеты сообщения диалога.</returns>
    public async Task GetDialogMessagesAsync(long dialogId, string account)
    {
        try
        {
            if (dialogId <= 0)
            {
                throw new InvalidOperationException("Id диалога не передан.");
            }

            var result = await _abstractGroupDialogMessagesService.GetObjectDialogMessagesAsync(dialogId, account);
            
            var connection = await GetConnectionCacheAsync();

            await Clients
                .Client(connection.ConnectionId)
                .SendAsync("getDialogMessages", result)
                .ConfigureAwait(false);
        }
        
        catch (Exception ex)
        {
            await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);
            
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
    
    /// <summary>
    /// Метод отправляет сообщение в очередь RabbitMQ.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    /// <param name="dialogId">Id диалога.</param>
    /// <param name="account">Аккаунт.</param>
    public async Task SendMessageToBackAsync(string? message, long dialogId, string account)
    {
        var userCode = Context.GetHttpContext()?.Request.Query["userCode"].ToString();
        var module = Enum.Parse<UserConnectionModuleEnum>(
            Context.GetHttpContext()?.Request.Query["module"].ToString()!);

        try
        {
            await _abstractGroupDialogMessagesService.SendMessageToQueueAsync(message, dialogId, account,
                    new Guid(userCode), module)
                .ConfigureAwait(false);
        }
        
        catch (Exception ex)
        {
            await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);
            
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод получает подключение из кэша Redis.
    /// </summary>
    /// <returns>Вернет объект подключения.</returns>
    /// <exception cref="InvalidOperationException">Если ошибка при формировании результата.</exception>
    private async Task<UserConnectionOutput?> GetConnectionCacheAsync()
    {
        var userCode = Context.GetHttpContext()?.Request.Query["userCode"].ToString();
        var module = Enum.Parse<UserConnectionModuleEnum>(
            Context.GetHttpContext()?.Request.Query["module"].ToString()!);
        var key = string.Concat(userCode + "_", module.ToString());
            
        var connection = await _connectionService.GetConnectionIdCacheAsync(key);

        if (string.IsNullOrWhiteSpace(connection?.ConnectionId))
        {
            throw new InvalidOperationException("Ошибка получения подключения пользователя из Redis. " +
                                                "Не удалось получить объекты группы абстрактной области чата.");
        }

        return connection;
    }

    #endregion
}