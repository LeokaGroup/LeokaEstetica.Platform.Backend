using LeokaEstetica.Platform.Base.Enums;
using LeokaEstetica.Platform.Core.Extensions;

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
        QueueTypeEnum.RefundsQueue,
        QueueTypeEnum.ScrumMasterAiMessage,
        QueueTypeEnum.ScrumMasterAiAnalysis,
        QueueTypeEnum.DialogMessages
    };

    /// <summary>
    /// Метод создает название очереди для объявления очереди в зависимости от среды окружения.
    /// <param name="queue">Переменная, к которой применяется расширение.</param>
    /// <param name="environment">Среда окружения.</param>
    /// <param name="queueType">Тип очереди, для которой нужно получить название.</param>
    /// </summary>
    /// <exception cref="ArgumentNullException">Если не передали название очереди.</exception>
    /// <exception cref="InvalidOperationException">Если передали неизвестный тип очереди.</exception>
    /// <returns>Название очереди.</returns>
    public static string CreateQueueDeclareNameFactory(this string queue, string environment,
        QueueTypeEnum queueType)
    {
        if (!_flags.Contains(queueType))
        {
            throw new InvalidOperationException($"Неизвестный тип очереди. QueueType: {queueType}");
        }

        // Если тип очереди заказов.
        if (queueType.HasFlag(QueueTypeEnum.OrdersQueue))
        {
            if (environment.Equals("Development"))
            {
                queue = string.Concat("Develop_", QueueTypeEnum.OrdersQueue.GetEnumDescription());
            }
        
            else if (environment.Equals("Staging"))
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
            if (environment.Equals("Development"))
            {
                queue = string.Concat("Develop_", QueueTypeEnum.RefundsQueue.GetEnumDescription());
            }
        
            else if (environment.Equals("Staging"))
            {
                queue = string.Concat("Test_", QueueTypeEnum.RefundsQueue.GetEnumDescription());
            }

            else
            {
                // Обычное название, как на проде.
                queue = QueueTypeEnum.RefundsQueue.GetEnumDescription();
            }
        }
        
        // Если тип очереди сообщения для нейросети Scrum Master AI.
        else if (queueType.HasFlag(QueueTypeEnum.ScrumMasterAiMessage))
        {
            if (environment.Equals("Development"))
            {
                queue = string.Concat("Develop_", QueueTypeEnum.ScrumMasterAiMessage.GetEnumDescription());
            }
        
            else if (environment.Equals("Staging"))
            {
                queue = string.Concat("Test_", QueueTypeEnum.ScrumMasterAiMessage.GetEnumDescription());
            }

            else
            {
                // Обычное название, как на проде.
                queue = QueueTypeEnum.ScrumMasterAiMessage.GetEnumDescription();
            }
        }

        return queue;
    }
}