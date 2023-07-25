namespace LeokaEstetica.Platform.Base.Abstractions.Repositories.Common;

// TODO: #10343304 Надо настроить регистрацию с таким типом в AutoFac.
public interface IUpdateDetachedEntitiesRepository<T> where T : class
{
    Task UpdateAsync(List<T> entities);
}