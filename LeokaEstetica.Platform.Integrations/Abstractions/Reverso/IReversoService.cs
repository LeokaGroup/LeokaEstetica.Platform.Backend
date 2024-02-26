using LeokaEstetica.Platform.Core.Enums;

namespace LeokaEstetica.Platform.Integrations.Abstractions.Reverso;

/// <summary>
/// Абстракция сервиса транслитера ReversoAPI <see cref="https://github.com/Overmiind/ReversoAPI/tree/master"/>.
/// </summary>
public interface IReversoService
{
    /// <summary>
    /// Метод переводит строку с одного языка на другой.
    /// Перевод зависит от переданных параметров.
    /// </summary>
    /// <param name="wordToTranslate">Слово подлежащее переводу.</param>
    /// <param name="langFrom">С какого языка переводить.</param>
    /// <param name="langTo">На какой язык переводить.</param>
    /// <returns>Транслированная на указанный язык строка.</returns>
    Task<string> TranslateTextRussianToEnglishAsync(string textToTranslate, TranslateLangTypeEnum langFrom,
        TranslateLangTypeEnum langTo);
}