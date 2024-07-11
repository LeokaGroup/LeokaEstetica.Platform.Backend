using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LeokaEstetica.Platform.Access.Abstractions.User;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Access.User;
using LeokaEstetica.Platform.Models.Dto.Output.User;
using Microsoft.IdentityModel.Tokens;

namespace LeokaEstetica.Platform.Access.Services.User;

/// <summary>
/// Класс реализует методы сервиса проверки доступа пользователей.
/// </summary>
internal sealed class AccessUserService : IAccessUserService
{
    private readonly IAccessUserRepository _accessUserRepository;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="accessUserRepository">Репозиторий доступа пользователя.</param>
    public AccessUserService(IAccessUserRepository accessUserRepository)
    {
        _accessUserRepository = accessUserRepository;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод проверяет блокировку пользователя по параметру, который передали.
    /// Поочередно проверяем по почте, номеру телефона.
    /// </summary>
    /// <param name="availableBlockedText">Почта или номер телефона для проверки блокировки.</param>
    /// <param name="isVkAuth">Признак блокировки через ВК.</param>
    /// <returns>Признак блокировки.</returns>
    public async Task<bool> CheckBlockedUserAsync(string availableBlockedText, bool isVkAuth)
    {
        var blockedUser = await _accessUserRepository.CheckBlockedUserAsync(availableBlockedText, isVkAuth);

        return blockedUser;
    }

    /// <summary>
    /// Метод проверяет, заполнена ли анкета пользователя.
    /// Если не заполнена, то запрещаем доступ к ключевому функционалу.
    /// </summary>
    /// <param name="userId">ID пользователя.</param>
    /// <returns>Признак проверки.</returns>
    public async Task<bool> IsProfileEmptyAsync(long userId)
    {
        var profile = await _accessUserRepository.IsProfileEmptyAsync(userId);

        if (string.IsNullOrEmpty(profile.UserProfile?.FirstName)
            || string.IsNullOrEmpty(profile.UserProfile?.LastName)
            || string.IsNullOrEmpty(profile.UserProfile?.Job)
            || string.IsNullOrEmpty(profile.UserProfile?.Aboutme)
            || !profile.UserIntents.Any()
            || !profile.UserSkills.Any())
        {
            return true;
        }

        return false;
    }
    
    /// <summary>
    /// Метод выдает токен пользователю, если он прошел авторизацию.
    /// </summary>
    /// <param name="email">Email.</param>
    /// <returns>Токен пользователя.</returns>
    public async Task<ClaimsIdentity> GetIdentityClaimAsync(string email)
    {
        var claims = new List<Claim>
        {
            new(ClaimsIdentity.DefaultNameClaimType, email)
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Token", 
            ClaimsIdentity.DefaultNameClaimType, 
            ClaimsIdentity.DefaultRoleClaimType);

        return await Task.FromResult(claimsIdentity);
    }
    
    /// <summary>
    /// Метод создает токен пользователю.
    /// </summary>
    /// <param name="claimsIdentity">Объект полномочий.</param>
    /// <returns>Строка токена.</returns>
    public async Task<string> CreateTokenFactoryAsync(ClaimsIdentity claimsIdentity)
    {
        var now = DateTime.UtcNow;
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            notBefore: now,
            claims: claimsIdentity.Claims,
            expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        return await Task.FromResult(encodedJwt);
    }
    
    /// <summary>
    /// Метод выдает токен пользователю, если он прошел авторизацию.
    /// </summary>
    /// <param name="vkUserId">VkUserId пользователя.</param>
    /// <returns>Токен пользователя.</returns>
    public async Task<ClaimsIdentity> GetIdentityClaimVkUserAsync(long vkUserId)
    {
        var claims = new List<Claim>
        {
            new(ClaimsIdentity.DefaultNameClaimType, vkUserId.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Token",
            ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);

        return await Task.FromResult(claimsIdentity);
    }
    
    /// <summary>
    /// Метод обновляет токен.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Новые данные авторизации.</returns>
    public async Task<UserSignInOutput> RefreshTokenAsync(string account)
    {
        var claim = await GetIdentityClaimAsync(account);
        var token = await CreateTokenFactoryAsync(claim);

        var result = new UserSignInOutput
        {
            Token = token
        };

        return await Task.FromResult(result);
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}