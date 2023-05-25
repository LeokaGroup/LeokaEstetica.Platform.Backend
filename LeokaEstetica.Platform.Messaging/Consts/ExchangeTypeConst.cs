namespace LeokaEstetica.Platform.Messaging.Consts;

/// <summary>
/// Класс описывает типы точек обмена кролика.
/// </summary>
internal static class ExchangeTypeConst
{
    public const string Direct = "direct";
    public const string Topic = "topic";
    public const string Fanout = "fanout";
    public const string Header = "headers";
}