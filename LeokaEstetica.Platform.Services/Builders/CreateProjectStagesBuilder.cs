using LeokaEstetica.Platform.Models.Enums;
using Enum = System.Enum;

namespace LeokaEstetica.Platform.Services.Builders;

/// <summary>
/// Класс билдера создает список стадий проекта для входного параметра.
/// </summary>
public static class CreateProjectStagesBuilder
{
    /// <summary>
    /// Список значений стадий проекта.
    /// </summary>
    private static readonly List<FilterProjectStageTypeEnum> _projectStages = new();

    /// <summary>
    /// Список, который будем выдавать по дефолту, если ничего не передали.
    /// </summary>
    private static readonly List<FilterProjectStageTypeEnum> _defaultProjectStages = new()
    {
        FilterProjectStageTypeEnum.None
    };

    /// <summary>
    /// Метод создает список стадий проекта из строки.
    /// </summary>
    /// <param name="employments">Входная строка для парсинга.</param>
    /// <returns>Список стадий проекта.</returns>
    public static List<FilterProjectStageTypeEnum> CreateProjectStagesResult(string employments)
    {
        if (string.IsNullOrEmpty(employments))
        {
            return _defaultProjectStages;
        }

        _projectStages.Clear();

        // Если есть разделитель, то можем разбить.
        if (employments.Contains(','))
        {
            var splitItems = employments.Split(",").Select(Enum.Parse<FilterProjectStageTypeEnum>);
            _projectStages.AddRange(splitItems);
        }

        // Иначе вставляем одно значение.
        else
        {
            var value = Enum.Parse<FilterProjectStageTypeEnum>(employments);
            _projectStages.Add(value);
        }

        return _projectStages;
    }
}