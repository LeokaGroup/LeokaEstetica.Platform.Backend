using System.Transactions;

namespace LeokaEstetica.Platform.Database.Abstractions.Connection;

/// <summary>
/// Интерфейс фабрики контекста транзакций.
/// </summary>
public interface ITransactionScopeFactory
{
    /// <summary>
    /// Создает новый экземпляр контекста транзакций.
    /// </summary>
    /// <returns> Новый экземпляр контекста транзакций. </returns>
    TransactionScope CreateTransactionScope();

    /// <summary>
    /// Создает новый экземпляр контекста транзакций с увеличенным таймаутом.
    /// </summary>
    /// <returns> Новый экземпляр контекста транзакций. </returns>
    TransactionScope CreateTransactionScopeIncrease();
}