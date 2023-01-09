using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Controllers.Builders;

/// <summary>
/// Класс билдера создает список занятостей для входного параметра.
/// </summary>
public static class CreateEmploymentsBuilder
{
    /// <summary>
    /// Список значений занятости.
    /// </summary>
    private static readonly List<FilterEmploymentTypeEnum> _employments = new();

    /// <summary>
    /// Список, который будем выдавать по дефолту, если ничего не передали.
    /// </summary>
    private static readonly List<FilterEmploymentTypeEnum> _defaultEmployments = new()
    {
        FilterEmploymentTypeEnum.None
    };

    /// <summary>
    /// Метод создает список занятостей из строки.
    /// </summary>
    /// <param name="employments">Входная строка для парсинга.</param>
    /// <returns>Список занятостей.</returns>
    public static List<FilterEmploymentTypeEnum> CreateEmploymentsResult(string employments)
    {
        if (string.IsNullOrEmpty(employments))
        {
            return _defaultEmployments;
        }

        _employments.Clear();

        // Если есть разделитель, то можем разбить.
        if (employments.Contains(','))
        {
            var splitItems = employments.Split(",").Select(Enum.Parse<FilterEmploymentTypeEnum>);
            _employments.AddRange(splitItems);
        }

        // Иначе вставляем одно значение.
        else
        {
            var value = Enum.Parse<FilterEmploymentTypeEnum>(employments);
            _employments.Add(value);
        }

        return _employments;
    }
}