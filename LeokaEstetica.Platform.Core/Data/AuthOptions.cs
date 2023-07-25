using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace LeokaEstetica.Platform.Core.Data;

public class AuthOptions
{
    public const string ISSUER = "MyAuthServer"; // Издатель токена.
    public const string AUDIENCE = "MyAuthClient"; // Потребитель токена.
    const string KEY = "mysupersecret_secretkey!123";   // Ключ для шифрования.
    public const int LIFETIME = 60; // Время жизни токена. Обычно ставят 10-15, но тут нужно больше.

    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
    }
}