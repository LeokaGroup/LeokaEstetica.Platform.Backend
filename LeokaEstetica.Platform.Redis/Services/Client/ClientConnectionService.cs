using LeokaEstetica.Platform.Redis.Abstractions.Client;
using LeokaEstetica.Platform.Redis.Abstractions.Connection;
using LeokaEstetica.Platform.Redis.Models.Chat;

namespace LeokaEstetica.Platform.Redis.Services.Client;

/// <summary>
/// Метод реализует методы сервиса клиентов подключений.
/// </summary>
internal sealed class ClientConnectionService : IClientConnectionService
{
    private readonly IConnectionService _connectionService;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="connectionService">Сервис подключений кэша.</param>
    public ClientConnectionService(IConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<string>> CreateClientsResultAsync(long? dialogId, long userId, string token)
    {
        var dialogRedis = await _connectionService.GetDialogMembersConnectionIdsCacheAsync(
            dialogId.GetValueOrDefault().ToString());
        
        var connection = await _connectionService.GetConnectionIdCacheAsync(token);

        // В кэше нет данных, будем добавлять текущего пользователя как первого.
        if (dialogRedis is null || !dialogRedis.Any())
        {
            // Добавляем текущего пользователя.
            dialogRedis = new List<DialogRedis>
            {
                new()
                {
                    Token = token,
                    ConnectionId = connection.ConnectionId,
                    UserId = userId
                }
            };

            await _connectionService.AddDialogMembersConnectionIdsCacheAsync(dialogId.GetValueOrDefault(),
                dialogRedis);
        }

        // Нашли в кэше, будем проверять актуальность ConnectionId.
        else
        {
            var isActual = dialogRedis.Any(x => x.ConnectionId.Equals(connection.ConnectionId));

            // Не актуален, обновим ConnectionId текущего пользователя.
            if (!isActual)
            {
                var client = dialogRedis.Find(x => x.UserId == userId);

                if (client is null)
                {
                    if (dialogRedis.Any())
                    {
                        dialogRedis.Add(new DialogRedis
                        {
                            Token = token,
                            ConnectionId = connection.ConnectionId,
                            UserId = userId
                        });
                    }

                    else
                    {
                        // Добавляем текущего пользователя.
                        dialogRedis = new List<DialogRedis>
                        {
                            new()
                            {
                                Token = token,
                                ConnectionId = connection.ConnectionId,
                                UserId = userId
                            }
                        };
                    }

                    await _connectionService.AddDialogMembersConnectionIdsCacheAsync(dialogId.GetValueOrDefault(),
                        dialogRedis);
                }

                else
                {
                    // Если не актуален ConnectionId, то обновим на актуальный.
                    if (!client.ConnectionId.Equals(connection.ConnectionId))
                    {
                        client.ConnectionId = connection.ConnectionId;
                    }

                    await _connectionService.AddDialogMembersConnectionIdsCacheAsync(dialogId.GetValueOrDefault(),
                        dialogRedis);
                }
            }
        }

        var clients = dialogRedis.Select(x => x.ConnectionId);

        return clients;
    }
}