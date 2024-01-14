using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Base.Abstractions.Connection;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Database")]

namespace LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;

/// <summary>
/// Базовый класс репозитория.
/// </summary>
internal abstract class BaseRepository
{
    /// <summary>
    /// Возвращает провайдер объектов класса <see cref="IConnectionProvider"/>.
    /// </summary>
    protected IConnectionProvider ConnectionProvider { get; }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="connectionProvider"> Провайдер объектов класса <see cref="IConnectionProvider"/>. </param>
    /// <exception cref="ArgumentNullException"> Возникает в случае, когда аргументы имеют значение null. </exception>
    protected BaseRepository(IConnectionProvider connectionProvider)
    {
        ConnectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }
}