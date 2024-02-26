using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Integrations.Abstractions.Reverso;
using Microsoft.Extensions.Logging;
using ReversoAPI;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Integrations.Services.Reverso;

/// <summary>
/// Класс реализует методы сервиса транслитера ReversoAPI <see cref="https://github.com/Overmiind/ReversoAPI/tree/master"/>.
/// </summary>
internal sealed class ReversoService : IReversoService
{
    private readonly ILogger<ReversoService> _logger;

    /// <summary>
    /// Конструктор.
    /// <param name="logger">Логгер.</param>
    /// </summary>
    public ReversoService(ILogger<ReversoService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string> TranslateTextRussianToEnglishAsync(string textToTranslate,
        TranslateLangTypeEnum langFrom, TranslateLangTypeEnum langTo)
    {
        try
        {
            using var reverso = new ReversoClient();
            var from = Enum.Parse<Language>(langFrom.ToString());
            var to = Enum.Parse<Language>(langTo.ToString());

            var translation = await reverso.Translation.GetAsync(textToTranslate, from, to);
            var result = translation.Translations.First().Value;

            return result;
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Ошибка перевода через транслитер." +
                                 $" TextToTranslate: {textToTranslate}. " +
                                 $"LangFrom: {langFrom}. " +
                                 $"LangTo: {langTo}.");
            throw;
        }
    }
}