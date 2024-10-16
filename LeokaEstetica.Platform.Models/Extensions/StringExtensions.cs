using System.Text;
using System.Text.RegularExpressions;

namespace LeokaEstetica.Platform.Models.Extensions;

/// <summary>
/// Класс расширений для строк.
/// Содержит некоторые одинаковые методы как в LeokaEstetica.Platform.Base.Extensions.StringExtensions
/// ради избежания референсов между слоями архитектуры, т.к. надо избегать ссылок из Models на Base.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Конвертирует строковое значение в snake-case.
    /// </summary>
    /// <param name="value"> Значение для конвертации. </param>
    /// <returns> Конвертированное значение. </returns>
    public static string ToSnakeCase(this string value)
    {
        return string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())).ToLower();
    }
    
    /// <summary>
    /// Метод переводит в PascalCase.
    /// </summary>
    /// <param name="s">Входная строка.</param>
    /// <returns>Измененная строка.</returns>
    public static string ToPascalCase(this string str)
    {
        // Найдите части слов, используя следующие правила:
        // 1. все строчные буквы, начинающиеся в начале, это слово
        // 2. Все заглавные буквы - это слово.
        // З. заглавные буквы, за которыми следуют строчные буквы - слово
        // 4. вся строка должна разложиться на слова согласно 1,2,3.
        var m = Regex.Match(str, "^(?<word>^[a-z]+|[A-Z]+|[A-Z][a-z]+)+$");
        var g = m.Groups["word"];

        // Берите каждое слово и преобразуйте его в TitleCase
        // для получения окончательного результата.  Обратите внимание на использование ToLower
        // перед ToTitleCase, потому что все заглавные буквы рассматриваются как сокращение.
        var t = Thread.CurrentThread.CurrentCulture.TextInfo;
        var sb = new StringBuilder();

        foreach (var c in g.Captures.Cast<Capture>())
        {
            sb.Append(t.ToTitleCase(c.Value.ToLower()));
        }

        return sb.ToString();
    }
    
    /// <summary>
    /// Метод переводит из snake_case в PascalCase.
    /// </summary>
    /// <param name="str">Строка дял перевода.</param>
    /// <returns>Измененная строка.</returns>
    public static string ToPascalCaseFromSnakeCase(this string str)
    {
        return string.Concat(str.Split('_').Select(Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase));
    }
}