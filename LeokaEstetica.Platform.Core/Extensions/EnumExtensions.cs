using System.ComponentModel;

namespace LeokaEstetica.Platform.Core.Extensions;

/// <summary>
/// Класс расширений для перечислений.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Метод получает description перечисления.
    /// </summary>
    /// <param name="enumValue">Значение, по которому можем получить description.</param>
    /// <returns>Description перечисления.</returns>
    public static string GetEnumDescription(this Enum enumValue)  
    {  
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());  
  
        var descriptionAttributes = (DescriptionAttribute[])fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false);  
  
        return descriptionAttributes?.Length > 0 ? descriptionAttributes[0].Description : enumValue.ToString();  
    }  
}