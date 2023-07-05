using System.Security.Cryptography;

namespace LeokaEstetica.Platform.Access.Helpers;

/// <summary>
/// Класс хелпера хэшей. 
/// </summary>
public static class HashHelper
{
    /// <summary>
    /// Метод хэширует пароль аналогично как это делает Identity.
    /// </summary>
    /// <param name="password">Исходный пароль без хэша.</param>
    /// <returns>Хэш пароля.</returns>
    public static string HashPassword(string password)
    {
        byte[] salt;
        byte[] buffer2;

        using (var bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
        {
            salt = bytes.Salt;
            buffer2 = bytes.GetBytes(0x20);
        }

        var dst = new byte[0x31];
        Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
        Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);

        return Convert.ToBase64String(dst);
    }
    
     /// <summary>
    /// Метод проверит пароли. 
    /// </summary>
    /// <param name="hashedPassword">Хэш пароля из БД.</param>
    /// <param name="password">Пароль пользователя пока еще без хэша и соли.</param>
    /// <returns>Флаг результата сравнения хэшей вместе с солями.</returns>
    public static bool VerifyHashedPassword(string hashedPassword, string password)
    {
        if (hashedPassword == null)
        {
            return false;
        }
        
        if (password == null)
        {
            return false;
        }
        
        var src = Convert.FromBase64String(hashedPassword);
        
        if ((src.Length != 0x31) || (src[0] != 0))
        {
            return false;
        }
        
        var dst = new byte[0x10];
        Buffer.BlockCopy(src, 1, dst, 0, 0x10);
        var buffer3 = new byte[0x20];
        Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
        
        using var bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8);
        var buffer4 = bytes.GetBytes(0x20);

        return AreHashesEqual(buffer3, buffer4);
    }

    /// <summary>
    /// Метод проверит хэши паролей вместе с солью.
    /// </summary>
    /// <param name="firstHash">Исодный хэш с солью из БД.</param>
    /// <param name="secondHash">Новый хэш с солью.</param>
    /// <returns>Флаг результата сравнения хэшей вместе с солями.</returns>
    private static bool AreHashesEqual(IReadOnlyList<byte> firstHash, IReadOnlyList<byte> secondHash)
    {
        try
        {
            var _minHashLength = firstHash.Count <= secondHash.Count ? firstHash.Count : secondHash.Count;
            var xor = firstHash.Count ^ secondHash.Count;

            for (var i = 0; i < _minHashLength; i++)
            {
                xor |= firstHash[i] ^ secondHash[i];
            }
        
            return 0 == xor;
        }
        
        // TODO: добавить логирование ошибок.
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}