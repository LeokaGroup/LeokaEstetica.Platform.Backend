using LeokaEstetica.Platform.Models.Dto.Output.Template;
using LeokaEstetica.Platform.Services.Builders.BuilderData;

namespace LeokaEstetica.Platform.Services.Builders.AgileObjectBuilder;

/// <summary>
/// Абстрактный класс строителя объекта Agile.
/// Объект Agile - это может быть задача, эпик, история, ошибка. Смотря что будет создаваться строителем.
/// </summary>
internal abstract class AgileObjectBuilder
{
    /// <summary>
    /// Данные необходимые для работы строителей. Зависимости и тд.
    /// </summary>
    protected internal AgileObjectBuilderData BuilderData;
    
    /// <summary>
    /// Построенная модель. Является результатом. Это может быть задача, эпик, спринт, ошибка.
    /// </summary>
    protected internal ProjectManagmentTaskOutput ProjectManagmentTask { get; set; }

    /// <summary>
    /// Метод создает результирующую модель.
    /// Наполнение строителями сразу не происходит. Она создается пустой.
    /// </summary>
    public Task CreateProjectManagmentTaskAsync()
    {
        ProjectManagmentTask = new ProjectManagmentTaskOutput
        {
            Executor = new Executor()
        };

        return Task.FromResult(ProjectManagmentTask);
    }
    
    /// <summary>
    /// Метод получает данные из БД. Эти данные нужны для работы строителя.
    /// </summary>
    /// <returns>Построенная модель.</returns>
    public abstract Task InitObjectAsync();
    
    /// <summary>
    /// Метод записывает название автора.
    /// </summary>
    public abstract Task FillAuthorNameAsync();
    
    /// <summary>
    /// Метод записывает название исполнителя.
    /// </summary>
    public abstract Task FillExecutorNameAsync();
    
    /// <summary>
    /// Метод записывает данные аватара.
    /// </summary>
    public abstract Task FillExecutorAvatarAsync();
    
    /// <summary>
    /// Метод записывает названия наблюдателей.
    /// </summary>
    public abstract Task FillWatcherNamesAsync();
    
    /// <summary>
    /// Метод записывает названия тегов.
    /// </summary>
    public abstract Task FillTagIdsAsync();
    
    /// <summary>
    /// Метод записывает название типа.
    /// </summary>
    public abstract Task FillTaskTypeNameAsync();
    
    /// <summary>
    /// Метод записывает название статуса.
    /// </summary>
    public abstract Task FillTaskStatusNameAsync();
    
    /// <summary>
    /// Метод записывает название резолюции.
    /// </summary>
    public abstract Task FillResolutionNameAsync();
    
    /// <summary>
    /// Метод записывает название приоритета.
    /// </summary>
    public abstract Task FillPriorityNameAsync();
    
    /// <summary>
    /// Метод записывает Id эпика и название эпика.
    /// </summary>
    public abstract Task FillEpicIdAndEpicNameAsync();

    /// <summary>
    /// Метод записывает Id спринта и название спринта.
    /// </summary>
    public abstract Task FillSprintIdAndSprintNameAsync();
}