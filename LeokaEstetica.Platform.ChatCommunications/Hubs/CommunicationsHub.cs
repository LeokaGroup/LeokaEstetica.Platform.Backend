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

            var enumValue = Enum.GetValues<AbstractScopeTypeEnum>().FirstOrDefault(x => (int)x == abstractScopeType);

            if (enumValue == AbstractScopeTypeEnum.Undefined)
            {
                throw new InvalidOperationException("Тип абстрактной области невалиден. " +
                                                    $"AbstractScopeType: {abstractScopeType}.");
            }

            // Получаем список групп объектов выбранной абстрактной области чата.
            var result = await _abstractGroupService.GetAbstractGroupObjectsAsync(abstractScopeId, enumValue,
                account);
                
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