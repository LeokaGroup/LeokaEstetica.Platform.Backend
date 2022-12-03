using LeokaEstetica.Platform.Base.Abstractions.Repositories;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Models.Entities.Configs;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Base.Repositories;

/// <summary>
/// Класс реализует методы репозитория для исключения параметров валидации, которые не нужно выдавать фронту.
/// </summary>
public sealed class ValidationExcludeErrorsRepository : IValidationExcludeErrorsRepository
{
    private readonly PgContext _pgContext;
    
    public ValidationExcludeErrorsRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    /// <summary>
    /// Метод получает список полей, которые нужно исключить.
    /// </summary>
    /// <returns>Список полей для исключения.</returns>
    public async Task<IEnumerable<ValidationColumnExcludeEntity>> ValidationColumnsExcludeAsync()
    {
        var result = await _pgContext.ValidationColumnsExclude
            .ToListAsync();

        return result;
    }
}