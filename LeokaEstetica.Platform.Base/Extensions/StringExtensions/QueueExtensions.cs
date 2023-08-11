using LeokaEstetica.Platform.Base.Enums;
using LeokaEstetica.Platform.Core.Extensions;
using Microsoft.Extensions.Configuration;

namespace LeokaEstetica.Platform.Base.Extensions.StringExtensions;

/// <summary>
/// Класс расширений строк очередей.
/// </summary>
public static class QueueExtensions
{
    /// <summary>
    /// Типы флагов очередей.
    /// </summary>
    private static readonly HashSet<QueueTypeEnum> _flags = new()
    {
        QueueTypeEnum.OrdersQueue,
        QueueTypeEnum.RefundsQueue
    };

    /// <summary>
    /// Метод создает название очереди для объявления очереди в зависимости от среды окружения.
    /// <param name="queue">Переменная, к которой применяется расширение.</param>
    /// <param name="configuration">Конфигурация.</param>
    /// <param name="queueType">Тип очереди, для которой нужно получить название.</param>
    /// </summary>
    /// <exception cref="ArgumentNullException">Если не передали название очереди.</exception>
    /// <exception cref="InvalidOperationException">Если передали неизвестный тип очереди.</exception>
    /// <returns>Название очереди.</returns>
    public static string CreateQueueDeclareNameFactory(this string queue, IConfiguration configuration,
        QueueTypeEnum queueType)
    {
        if (queue is null)
        {
            throw new ArgumentNullException("Не передано название очереди.");
        }

        if (!_flags.Contains(queueType))
        {
            throw new InvalidOperationException($"Неизвестный тип очереди. QueueType: {queueType}");
        }

        // Если тип очереди заказов.
        if (queueType.HasFlag(QueueTypeEnum.OrdersQueue))
        {
            if (configuration["Environment"].Equals("Development"))
            {
                queue = string.Concat("Develop_", QueueTypeEnum.OrdersQueue.GetEnumDescription());
            }
        
            else if (configuration["Environment"].Equals("Staging"))
            {
                queue = string.Concat("Test_", QueueTypeEnum.OrdersQueue.GetEnumDescription());
            }

            else
            {
                // Обычное название, как на проде.
                queue = QueueTypeEnum.OrdersQueue.GetEnumDescription();
            }
        }
        
        // Если тип очереди возвратов.
        else if (queueType.HasFlag(QueueTypeEnum.RefundsQueue))
        {
            if (configuration["Environment"].Equals("Development"))
            {
                queue = string.Concat("Develop_", QueueTypeEnum.RefundsQueue.GetEnumDescription());
            }
        
            else if (configuration["Environment"].Equals("Staging"))
            {
                queue = string.Concat("Test_", QueueTypeEnum.RefundsQueue.GetEnumDescription());
            }

            else
            {
                // Обычное название, как на проде.
                queue = QueueTypeEnum.RefundsQueue.GetEnumDescription();
            }
        }

        return queue;
    }
}