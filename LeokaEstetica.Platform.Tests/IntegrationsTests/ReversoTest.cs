using LeokaEstetica.Platform.Core.Enums;
using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.IntegrationsTests;

[TestFixture]
internal class ReversoTest : BaseServiceTest
{
    [Test]
    public async Task TranslateRussianToEnglishAsyncTest()
    {
        var result = await ReversoService.TranslateTextRussianToEnglishAsync("Тестовый текст для перевода",
            TranslateLangTypeEnum.Russian, TranslateLangTypeEnum.English);
        
        NotNull(result);
        IsNotEmpty(result);
        That(result, Is.EqualTo("Test text for translation"));
    }
}