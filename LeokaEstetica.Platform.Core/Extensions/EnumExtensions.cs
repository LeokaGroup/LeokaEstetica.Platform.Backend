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
    
    /// <summary>
    /// Если член перечесления содержит атрибут Description, возвращает его значение.
    /// </summary>
    /// <typeparam name="TEnum"> Тип перечисления. </typeparam>
    /// <param name="instance"> Экземпляр перечисления. </param>
    /// <param name="isExceptionSuppressed"> Если значение атрибута не найдено, подавить исключение и вернуть null. </param>
    /// <returns> Значение атрибута EnumMember </returns>
    /// <exception cref="InvalidEnumArgumentException"> Возникает, если значение не найдено </exception>
    public static string? GetDescription<TEnum>(this TEnum instance, bool isExceptionSuppressed = false)
        where TEnum : Enum
    {
        var enumType = typeof(TEnum);
        var name = Enum.GetName(typeof(TEnum), instance);

        if (name is null)
        {
            throw new InvalidEnumArgumentException($"Член {nameof(instance)} пречисления {typeof(TEnum).Name} не найден");
        }

        var descriptionAttribute = (enumType
                .GetField(name)
                .GetCustomAttributes(typeof(DescriptionAttribute), true) as DescriptionAttribute[])
            .SingleOrDefault();

        if (isExceptionSuppressed)
        {
            return descriptionAttribute?.Description;
        }

        if (descriptionAttribute is null)
        {
            throw new InvalidEnumArgumentException($"Член {nameof(instance)} перечисления {typeof(TEnum).Name} не содержит атрибут Description");
        }

        return descriptionAttribute.Description;
    }
}