using System.Runtime.CompilerServices;
using System.Transactions;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.ProjectManagment.Documents")]

namespace LeokaEstetica.Platform.Base.Factors;

/// <summary>
/// Класс реализует методы факторки транзакций.
/// </summary>
internal sealed class TransactionScopeFactory : ITransactionScopeFactory
{
    /// <summary>
    /// Метод создает новый экземпляр контекста транзакций.
    /// </summary>
    /// <returns> Новый экземпляр контекста транзакций. </returns>
    public TransactionScope CreateTransactionScope()
    {
        return new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            },
            TransactionScopeAsyncFlowOption.Enabled);
    }
}