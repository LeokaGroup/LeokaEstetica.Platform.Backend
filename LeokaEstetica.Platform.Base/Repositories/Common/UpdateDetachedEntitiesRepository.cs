using LeokaEstetica.Platform.Base.Abstractions.Repositories.Common;
using LeokaEstetica.Platform.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Base.Repositories.Common;

// TODO: #10343304 Надо настроить регистрацию с таким типом в AutoFac.
public class UpdateDetachedEntitiesRepository<T> : IUpdateDetachedEntitiesRepository<T> where T : class
{
    private readonly PgContext _pgContext;

    public UpdateDetachedEntitiesRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    public async Task UpdateAsync(List<T> entities)
    {
        // Проводим все эти манипуляции, чтобы избежать ошибки при обновлении замечаний, которые уже были внесены.
        foreach (var e in entities)
        {
            var local = _pgContext.Set<T>()
                .Local
                .FirstOrDefault(entry => entry.GetType().GetProperty("RemarkId") == e.GetType().GetProperty("RemarkId"));

            // Если локальная сущность != null.
            if (local != null)
            {
                // Отсоединяем контекст устанавливая флаг Detached.
                _pgContext.Entry(local).State = EntityState.Detached;
            }
            
            // Проставляем обновляемой сущности флаг Modified.
            _pgContext.Entry(e).State = EntityState.Modified;
        }
        
        await _pgContext.SaveChangesAsync();
    }
}