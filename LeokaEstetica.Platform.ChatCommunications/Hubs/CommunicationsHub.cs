using LeokaEstetica.Platform.Communications.Abstractions;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Communications.Output;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Redis.Abstractions.Connection;
using Microsoft.AspNetCore.SignalR;
using Enum = System.Enum;

namespace LeokaEstetica.Platform.Communications.Hubs;

/// <summary>
/// Класс хаба модуля коммуникаций.
/// </summary>
internal sealed class CommunicationsHub : Hub, IAbstractScopeService
{
    private readonly IAbstractScopeService _abstractScopeService;
    private readonly ILogger<CommunicationsHub> _logger;
    private readonly IDiscordService _discordService;
    private readonly IConnectionService _connectionService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="abstractScopeService">Сервис абстрактных областей чата.</param>
    /// <param name="logger">Логгер.</param>
    /// <param name="discordService">Сервис уведомлений дискорда.</param>
    /// <param name="connectionService">Сервис подключений Redis.</param>
    public CommunicationsHub(IAbstractScopeService abstractScopeService,
        ILogger<CommunicationsHub> logger,
        IDiscordService discordService,
        IConnectionService connectionService)
    {
        _abstractScopeService = abstractScopeService;
        _logger = logger;
        _discordService = discordService;
        _connectionService = connectionService;
    }

    #region Публичные методы.

    /// <inheritdoc />
    public override async Task OnConnectedAsync()
    {
        var userCode = Context.GetHttpContext()?.Request.Query["userCode"].ToString();
        var module = Enum.Parse<UserConnectionModuleEnum>(
            Context.GetHttpContext()?.Request.Query["module"].ToString()!);

        if (!string.IsNullOrEmpty(userCode))
        {
            await _connectionService.AddConnectionIdCacheAsync(userCode, Context.ConnectionId, module)
                .ConfigureAwait(false);
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
    /// <returns>Возвращает через сокеты cписок абстрактных областей чата.</returns>
    public async Task GetScopesAsync(string account)
    {
        try
        {
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

            // Используем как прокси-метод.
            var result = await GetAbstractScopesAsync(account);

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

    /// <inheritdoc />
    public async Task<IEnumerable<AbstractScopeOutput>> GetAbstractScopesAsync(string account)
    {
        var result = await _abstractScopeService.GetAbstractScopesAsync(account);

        return result;
    }

    #endregion

    #region Приватные методы.

    #endregion
}