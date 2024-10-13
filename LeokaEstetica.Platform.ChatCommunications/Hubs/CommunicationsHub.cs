using LeokaEstetica.Platform.Communications.Abstractions;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
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

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="abstractScopeService">Сервис абстрактных областей чата.</param>
    /// <param name="logger">Логгер.</param>
    /// <param name="discordService">Сервис уведомлений дискорда.</param>
    /// <param name="connectionService">Сервис подключений Redis.</param>
    /// <param name="abstractGroupService">Сервис групп абстрактной области.</param>
    public CommunicationsHub(IAbstractScopeService abstractScopeService,
        ILogger<CommunicationsHub> logger,
        IDiscordService discordService,
        IConnectionService connectionService,
        IAbstractGroupService abstractGroupService)
    {
        _abstractScopeService = abstractScopeService;
        _logger = logger;
        _discordService = discordService;
        _connectionService = connectionService;
        _abstractGroupService = abstractGroupService;
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
            
            var userCode = Context.GetHttpContext()?.Request.Query["userCode"].ToString();
            var module = Enum.Parse<UserConnectionModuleEnum>(
                Context.GetHttpContext()?.Request.Query["module"].ToString()!);
            var key = string.Concat(userCode + "_", module.ToString());
            
            var connection = await _connectionService.GetConnectionIdCacheAsync(key);

            if (string.IsNullOrWhiteSpace(connection?.ConnectionId))
            {
                throw new InvalidOperationException("Ошибка получения подключения пользователя из Redis. " +
                                                    "Не удалось получить абстрактные области чата.");
            }

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
    /// </summary>
    /// <param name="abstractScopeId">Id выбранной абстрактной области чата.</param>
    /// <param name="abstractScopeType">Тип выбранной абстрактной области чата.</param>
    /// <param name="account">Аккаунт.</param>
    /// <exception cref="InvalidOperationException">Если ошибка валидации.</exception>
    /// <returns>Возвращает через сокеты группы объектов выбранной абстрактной области чата.</returns>
    public async Task GetScopeGroupObjectsAsync(long abstractScopeId, int abstractScopeType,
        string account)
    {
        try
        {
            if (abstractScopeId <= 0)
            {
                throw new InvalidOperationException("Id абстрактной области невалиден. " +
                                                    $"abstractScopeId: {abstractScopeId}.");
            }

            var enumValue = AbstractScopeTypeEnum.Undefined;

            foreach (var ev in Enum.GetValues<AbstractScopeTypeEnum>())
            {
                if ((int)ev == abstractScopeType)
                {
                    enumValue = ev;
                }
            }

            if (enumValue == AbstractScopeTypeEnum.Undefined)
            {
                throw new InvalidOperationException("Тип абстрактной области невалиден. " +
                                                    $"AbstractScopeType: {abstractScopeType}.");
            }

            // Получаем список групп объектов выбранной абстрактной области чата.
            var result = await _abstractGroupService.GetAbstractGroupObjectsAsync(abstractScopeId, enumValue,
                account);
                
            var userCode = Context.GetHttpContext()?.Request.Query["userCode"].ToString();
            var module = Enum.Parse<UserConnectionModuleEnum>(
                Context.GetHttpContext()?.Request.Query["module"].ToString()!);
            var key = string.Concat(userCode + "_", module.ToString());
            
            var connection = await _connectionService.GetConnectionIdCacheAsync(key);

            if (string.IsNullOrWhiteSpace(connection?.ConnectionId))
            {
                throw new InvalidOperationException("Ошибка получения подключения пользователя из Redis. " +
                                                    "Не удалось получить абстрактные области чата.");
            }

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

    #endregion

    #region Приватные методы.

    #endregion
}