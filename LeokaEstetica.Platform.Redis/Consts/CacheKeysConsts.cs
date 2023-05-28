namespace LeokaEstetica.Platform.Redis.Consts;

/// <summary>
/// Класс описывает ключи для кэша.
/// </summary>
public static class CacheKeysConsts
{
    /// <summary>
    /// Ключ для удаления аккаунтов пользователей.
    /// </summary>
    public const string DEACTIVATE_USER_ACCOUNTS = "DeactivatedUsers:Accounts";

    /// <summary>
    /// Ключ хранения заказов в кэше.
    /// </summary>
    public const string ORDER_CACHE = "OrdersCache:";
}