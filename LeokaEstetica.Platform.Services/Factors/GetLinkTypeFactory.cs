using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Models.Enums;
using Enum = System.Enum;

namespace LeokaEstetica.Platform.Services.Factors;

/// <summary>
/// Класс факторки со списком типов связей задач.
/// </summary>
public static class GetLinkTypeFactory
{
    /// <summary>
    /// Метод получает список типов связей задач.
    /// </summary>
    /// <returns>Список типов связей задач.</returns>
    public static async Task<IEnumerable<KeyValuePair<string, string>>> GetLinkTypesAsync()
    {
        var result = new List<KeyValuePair<string, string>>();
        
        foreach (var lt in Enum.GetValues(typeof(LinkTypeEnum)))
        {
            result.Add(new KeyValuePair<string, string>(lt.ToString(), ((LinkTypeEnum)lt).GetEnumDescription()));
        }

        return await Task.FromResult(result);
    }
}