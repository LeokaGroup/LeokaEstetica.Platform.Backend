using System.Transactions;
using LeokaEstetica.Platform.Database.Abstractions.Connection;

namespace LeokaEstetica.Platform.Database.Factors;

/// <summary>
/// Фабрика контекста транзакций.
/// </summary>
internal class TransactionScopeFactory : ITransactionScopeFactory
{
    /// <summary>
    /// Создает новый экземпляр контекста транзакций.
    /// </summary>
    /// <returns> Новый экземпляр контекста транзакций. </returns>
    public TransactionScope CreateTransactionScope()
    {
        return new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);
    }

    /// <summary>
    /// Создает новый экземпляр контекста транзакций с увеличенным таймаутом.
    /// </summary>
    /// <returns> Новый экземпляр контекста транзакций. </returns>
    public TransactionScope CreateTransactionScopeIncrease()
    {
        return new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.FromHours(1) },
            TransactionScopeAsyncFlowOption.Enabled);
    }
}