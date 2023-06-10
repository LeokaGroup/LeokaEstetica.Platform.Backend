using System.Runtime.CompilerServices;
using Quartz;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Backend")]

namespace LeokaEstetica.Platform.WorkerServices.Jobs.RabbitMq;

/// <summary>
/// Класс джобы консьюмера возвратов кролика.
/// </summary>
[DisallowConcurrentExecution]
internal sealed class RefundsJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        
    }
}