using LeokaEstetica.Platform.Processing.Enums;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.CommerceTests;

[TestFixture]
internal class CheckPaymentConfirmationStatusTest : BaseServiceTest
{
    /// <summary>
    /// Тест проверяет парсинг статуса из ПС дял подтверждения заказа.
    /// </summary>
    [Test]
    public async Task CheckPaymentConfirmationStatusUKassaTest()
    {
        var status = PaymentStatus.GetPaymentStatusBySysName("waiting_for_capture");
        
        // Должен быть статус, который допускает подтвердить платеж в ПС.
        Assert.That(status, Is.EqualTo(PaymentStatusEnum.WaitingForCapture));

        await Task.CompletedTask;
    }
}