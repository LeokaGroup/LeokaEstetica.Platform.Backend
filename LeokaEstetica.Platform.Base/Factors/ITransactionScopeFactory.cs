using System.Transactions;

namespace LeokaEstetica.Platform.Base.Factors;

/// <summary>
/// Абстракция факторки контекста транзакций.
/// </summary>
public interface ITransactionScopeFactory
{
    /// <summary>
    /// Метод создает новый экземпляр контекста транзакций.
    /// </summary>
    /// <returns> Новый экземпляр контекста транзакций. </returns>
    TransactionScope CreateTransactionScope();
}