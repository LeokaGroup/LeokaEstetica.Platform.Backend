using System.Data;
using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Base.Extensions.StringExtensions;
using LeokaEstetica.Platform.Base.Factors;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Models.Dto.Output.Roles;
using LeokaEstetica.Platform.Models.Dto.Output.User;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.Profile;
using LeokaEstetica.Platform.Models.Entities.Ticket;
using LeokaEstetica.Platform.Models.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Base.Repositories.User;

/// <summary>
/// Класс реализует методы репозитория пользователей.
/// </summary>
internal sealed class UserRepository : BaseRepository, IUserRepository
{
    private readonly PgContext _pgContext;
    private readonly ILogger<UserRepository> _logger;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    /// <param name="logger">Сервис логгера.</param>
    public UserRepository(PgContext pgContext,
        ILogger<UserRepository> logger,
        IConfiguration configuration,
        IConnectionProvider connectionProvider) : base(connectionProvider)
    {
        _pgContext = pgContext;
        _logger = logger;
        _configuration = configuration;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод сохраняет нового пользователя в базу.
    /// </summary>
    /// <param name="user">Данные пользователя для добавления.</param>
    /// <returns>Id пользователя.</returns>
    public async Task<long> AddUserAsync(UserEntity user)
    {
        await _pgContext.Users.AddAsync(user);
        await _pgContext.SaveChangesAsync();

        return user.UserId;
    }

    /// <summary>
    /// Метод находит пользователя по его UserId.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Данные пользователя.</returns>
    public async Task<UserEntity> GetUserByUserIdAsync(long userId)
    {
        UserEntity result;
        try
        {
            result = await _pgContext.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }
        
        // TODO: При dispose PgContext пересоздаем датаконтекст и пробуем снова.
        catch (ObjectDisposedException _)
        {
            var pgContext = CreateNewPgContextFactory.CreateNewPgContext(_configuration);
            result = await pgContext.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        return result;
    }

    /// <summary>
    /// Метод проверет существование пользователя по email в базе.
    /// </summary>
    /// <param name="email">Email пользователя.</param>
    /// <returns>Флаг проверки.</returns>
    public async Task<bool> CheckUserByEmailAsync(string email)
    {
        var result = await _pgContext.Users
            .AnyAsync(u => u.Email.Equals(email));

        return result;
    }

    /// <summary>
    /// Метод запишет код подтверждения пользователю.
    /// </summary>
    /// <param name="code">Код подтверждения, который мы отправили пользователю на почту.</param>
    /// <param name="userId">UserId.</param>
    public async Task SetConfirmAccountCodeAsync(Guid code, long userId)
    {
        var user = await GetUserByUserIdAsync(userId);

        if (user is null)
        {
            throw new NotFoundUserByIdException(userId);
        }
        
        user.ConfirmEmailCode = code;
        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод подтверждает аккаунт пользователя по коду, который ранее был отправлен пользователю на почту и записан в БД.
    /// </summary>
    /// <param name="code">Код подтверждения.</param>
    /// <returns>Статус подтверждения.</returns>
    public async Task<bool> ConfirmAccountAsync(Guid code)
    {
        var user = await _pgContext.Users.FirstOrDefaultAsync(u => u.ConfirmEmailCode.Equals(code));

        if (user is null)
        {
            throw new InvalidOperationException($"Не удалось подтвердить почту пользователя по коду {code}!");
        }

        user.EmailConfirmed = true;
        await _pgContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Метод получает хэш пароля для проверки пользователя.
    /// </summary>
    /// <param name="email">Почта.</param>
    /// <returns>Хэш пароля.</returns>
    public async Task<string> GetPasswordHashByEmailAsync(string email)
    {
        var result = await _pgContext.Users
            .Where(u => u.Email.Equals(email))
            .Select(u => u.PasswordHash)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод находит Id пользователя по его почте.
    /// </summary>
    /// <param name="account">Почта пользователя.</param>
    /// <returns>Id пользователя.</returns>
    public async Task<long> GetUserByEmailAsync(string account)
    {
        var result = await _pgContext.Users
            .Where(u => u.Email.Equals(account))
            .Select(u => u.UserId)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetUserEmailByUserIdsAsync(IEnumerable<long> userIds)
    {
        var result = await _pgContext.Users
            .Where(u => userIds.Contains(u.UserId))
            .Select(u => u.Email)
            .ToListAsync();

        return result;
    }

    /// <inheritdoc />
    public async Task<IDictionary<string, long>> GetUserByEmailsAsync(IEnumerable<string?> accounts)
    {
        var result = await _pgContext.Users
            .Where(u => accounts.Contains(u.Email))
            .Select(u => new
            {
                u.UserId,
                u.Email
            })
            .ToDictionaryAsync(k => k.Email, v => v.UserId);

        return result;
    }

    /// <summary>
    /// Метод получает код пользователя по его почте.
    /// </summary>
    /// <param name="email">Почта.</param>
    /// <returns>Хэш пароля.</returns>
    public async Task<Guid> GetUserCodeByEmailAsync(string email)
    {
        var result = await _pgContext.Users
            .Where(u => u.Email.Equals(email))
            .Select(u => u.UserCode)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод получает код пользователя по его VkUserId.
    /// </summary>
    /// <param name="vkUserId">VkUserId пользователя.</param>
    /// <returns>Хэш пароля.</returns>
    public async Task<Guid> GetUserCodeByVkUserIdAsync(long vkUserId)
    {
        var result = await _pgContext.Users
            .Where(u => u.VkUserId == vkUserId)
            .Select(u => u.UserCode)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод получает недостающую информацию профиля по UserId.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Основные данные профиля.</returns>
    public async Task<UserInfoOutput> GetUserPhoneEmailByUserIdAsync(long userId)
    {
        try
        {
            var result = await _pgContext.Users
                .Where(u => u.UserId == userId)
                .Select(u => new UserInfoOutput
                {
                    PhoneNumber = u.PhoneNumber,
                    Email = u.Email
                })
                .FirstOrDefaultAsync();

            return result;
        }
        
        // TODO: При dispose PgContext пересоздаем датаконтекст и пробуем снова.
        catch (ObjectDisposedException _)
        {
            var pgContext = CreateNewPgContextFactory.CreateNewPgContext(_configuration);
            var result = await pgContext.Users
                .Where(u => u.UserId == userId)
                .Select(u => new UserInfoOutput
                {
                    PhoneNumber = u.PhoneNumber,
                    Email = u.Email
                })
                .FirstOrDefaultAsync();

            return result;
        }
    }

    /// <summary>
    /// Метод сохраняет данные для таблицы пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="phone">Номер телефона.</param>
    /// <param name="email">Email.</param>
    public async Task<SaveUserProfileDataOutput> SaveUserDataAsync(long userId, string? phone, string email)
    {
        var user = await GetUserByUserIdAsync(userId);
        user.PhoneNumber = phone;
        user.Email = email;
        await _pgContext.SaveChangesAsync();

        var result = new SaveUserProfileDataOutput
        {
            IsEmailChanged = !user.Email.Equals(email)
        };

        return result;
    }

    /// <summary>
    /// Метод находит логин и почту пользователей по почте или логину пользователя.
    /// </summary>
    /// <param name="searchText">Текст, по которому надо искать.</param>
    /// <returns>Список пользователей.</returns>
    public async Task<List<UserEntity>?> GetUserByEmailOrLoginAsync(string searchText)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@searchText", searchText);

        var query = "SELECT u.\"UserId\", " +
                    "u.\"Email\", " +
                    "u.\"Login\" " +
                    "FROM dbo.\"Users\" AS u " +
                    "WHERE (u.\"Email\" ILIKE @searchText ||'%' " +
                    "OR u.\"Login\" ILIKE @searchText || '%')";

        var result = (await connection.QueryAsync<UserEntity>(query, parameters))?.AsList();

        return result;
    }

    /// <summary>
    /// Метод находит Id пользователя по почте или логину пользователя.
    /// </summary>
    /// <param name="searchText">Текст, по которому надо искать.</param>
    /// <returns>Id пользователя.</returns>
    public async Task<long> GetUserIdByEmailOrLoginAsync(string searchText)
    {
        var result = await _pgContext.Users
            .Where(u => u.Email.Contains(searchText) 
                        || u.Login.Contains(searchText))
            .Select(u => u.UserId)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод получает словарь кодов пользователей.
    /// </summary>
    /// <returns>Словарь кодов пользователей.</returns>
    public async Task<Dictionary<long, Guid>> GetUsersCodesAsync()
    {
        var result = await _pgContext.Users.ToDictionaryAsync(k => k.UserId, v => v.UserCode);

        return result;
    }

    /// <summary>
    /// Метод получает словарь кодов пользователей, Id которых передали.
    /// </summary>
    /// <returns>Словарь кодов пользователей.</returns>
    public async Task<Dictionary<long, Guid>> GetUsersCodesByUserIdsAsync(IEnumerable<long> userIds)
    {
        Dictionary<long, Guid>? result;

        try
        {
            result = await _pgContext.Users
                .Where(u => userIds.Contains(u.UserId))
                .ToDictionaryAsync(k => k.UserId, v => v.UserCode);
        }
       
        // TODO: При dispose PgContext пересоздаем датаконтекст и пробуем снова.
        catch (ObjectDisposedException _)
        {
            var pgContext = CreateNewPgContextFactory.CreateNewPgContext(_configuration);

            result = await pgContext.Users
                .Where(u => userIds.Contains(u.UserId))
                .ToDictionaryAsync(k => k.UserId, v => v.UserCode);
        }

        return result;
    }

    /// <summary>
    /// Метод получает список пользователей.
    /// </summary>
    /// <returns>Список пользователей.</returns>
    public async Task<List<UserEntity>> GetAllAsync()
    {
        var result = await _pgContext.Users.ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод находит Id пользователя по его коду.
    /// </summary>
    /// <param name="userCode">Код пользователя.</param>
    /// <returns>Id пользователя.</returns>
    public async Task<long> GetUserIdByCodeAsync(Guid userCode)
    {
        var result = await _pgContext.Users
            .Where(u => u.UserCode == userCode)
            .Select(u => u.UserId)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод находит Id пользователя по его Email.
    /// </summary>
    /// <param name="email">Email пользователя.</param>
    /// <returns>Id пользователя.</returns>
    public async Task<long> GetUserIdByEmailAsync(string email)
    {
        var result = await _pgContext.Users
            .Where(u => u.Email.Equals(email))
            .Select(u => u.UserId)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод находит Id пользователя по его номеру телефона.
    /// </summary>
    /// <param name="phoneNumber">Номер телефона пользователя.</param>
    /// <returns>Id пользователя.</returns>
    public async Task<long> GetUserIdByPhoneNumberAsync(string phoneNumber)
    {
        var result = await _pgContext.Users
            .Where(u => u.PhoneNumber.Equals(phoneNumber))
            .Select(u => u.UserId)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод находит Id пользователя по его логину.
    /// </summary>
    /// <param name="phoneNumber">Логин пользователя.</param>
    /// <returns>Id пользователя.</returns>
    public async Task<long> GetUserIdByLoginAsync(string login)
    {
        var result = await _pgContext.Users
            .Where(u => u.Login.Equals(login))
            .Select(u => u.UserId)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод проверет существование пользователя по VkUserId в базе.
    /// </summary>
    /// <param name="userId">VkUserId пользователя.</param>
    /// <returns>Флаг проверки.</returns>
    public async Task<bool> CheckUserByVkUserIdAsync(long userId)
    {
        var result = await _pgContext.Users
            .AnyAsync(u => u.VkUserId == userId);

        return result;
    }

    /// <summary>
    /// Метод находит Id пользователя по vk id.
    /// </summary>
    /// <param name="vkId">Id вконтакте.</param>
    /// <returns>Id пользователя.</returns>
    public async Task<long> GetUserIdByVkIdAsync(long vkId)
    {
        var result = await _pgContext.Users
            .Where(u => u.VkUserId == vkId)
            .Select(u => u.UserId)
            .FirstOrDefaultAsync();

        return result;
    }
    
    /// Метод проставляет пользователям метку к удалению аккаунтов.
    /// </summary>
    /// <param name="users">Список пользователей, которых предупредим.</param>
    public async Task SetMarkDeactivateAccountsAsync(List<UserEntity> users)
    {
        _pgContext.Users.UpdateRange(users);
        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод удаляет аккаунты пользователей.
    /// </summary>
    /// <param name="users">Список пользователей, которых удаляем.</param>
    /// <param name="profileItems">Список анкет пользователей, которых удаляем.</param>
    /// <param name="profileItems">Список анкет пользователей на модерации, которых удаляем.</param>
    /// <param name="ticketsMembersItems">Список участников тикетов.</param>
    /// <param name="ticketsMessagesItems">Список сообщений тикетов, которые удаляем.</param>
    public async Task DeleteDeactivateAccountsAsync(List<UserEntity> users, List<ProfileInfoEntity> profileItems,
        List<ModerationResumeEntity> moderationResumes, List<TicketMemberEntity> ticketsMembersItems,
        List<TicketMessageEntity> ticketsMessagesItems)
    {
        _logger.LogInformation($"Начали удаление анкет пользователей: {JsonConvert.SerializeObject(users)}.");
        
        var transaction = await _pgContext.Database
            .BeginTransactionAsync(IsolationLevel.ReadCommitted);

        try
        {
            // Находим и удаляем все анкеты пользователей на модерации.
            _pgContext.ModerationResumes.RemoveRange(moderationResumes);
            await _pgContext.SaveChangesAsync();
            
            // Находим и удаляем все анкеты пользователей.
            _pgContext.ProfilesInfo.RemoveRange(profileItems);
            await _pgContext.SaveChangesAsync();
            
            // Удаляем участников тикетов.
            _pgContext.TicketMembers.RemoveRange(ticketsMembersItems);

            // Удаляем сообщения тикетов.
            _pgContext.TicketMessages.RemoveRange(ticketsMessagesItems);

            // Удаляем самих пользователей.
            _pgContext.Users.RemoveRange(users);
            await _pgContext.SaveChangesAsync();
            
            await transaction.CommitAsync();
        }
        
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        _logger.LogInformation("Закончили удаление анкет пользователей.");
    }

    /// <summary>
    /// Метод находит Id пользователя по его Id анкеты.
    /// </summary>
    /// <param name="profileInfoId">Id анкеты.</param>
    /// <returns>Id пользователя.</returns>
    public async Task<long> GetUserIdByProfileInfoIdAsync(long profileInfoId)
    {
        var result = await _pgContext.ProfilesInfo
            .Where(u => u.ProfileInfoId == profileInfoId)
            .Select(u => u.UserId)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод находит аккаунт пользователя по его Id анкеты.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Аккаунт пользователя.</returns>
    public async Task<string> GetUserAccountByUserIdAsync(long userId)
    {
        var result = await _pgContext.Users
            .Where(u => u.UserId == userId)
            .Select(u => u.Email)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод находит Id анкеты по Id пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Id анкеты.</returns>
    public async Task<long> GetProfileInfoIdByUserIdAsync(long userId)
    {
        var result = await _pgContext.ProfilesInfo
            .Where(u => u.UserId == userId)
            .Select(u => u.ProfileInfoId)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод получает дату начала подписки и дату окончания подписки пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Дата начала и дата окончания подписки.</returns>
    public async Task<(DateTime? StartDate, DateTime? EndDate)> GetUserSubscriptionUsedDateAsync(long userId)
    {
        (DateTime? StartDate, DateTime? EndDate) result = (null, null);
        var item = await _pgContext.Users
            .Where(u => u.UserId == userId)
            .Select(u => new
            {
                u.SubscriptionStartDate,
                u.SubscriptionEndDate
            })
            .FirstOrDefaultAsync();

        if (item is null)
        {
            return result;
        }

        result.StartDate = item.SubscriptionStartDate;
        result.EndDate = item.SubscriptionEndDate;

        return result;
    }

    /// <summary>
    /// Метод восстанавливает пароль создавая новый.
    /// </summary>
    /// <param name="passwordHash">Хэш пароля.</param>
    /// <param name="userId">Id пользователя.</param>
    public async Task RestoreUserPasswordAsync(string passwordHash, long userId)
    {
        var user = await _pgContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

        if (user is null)
        {
            throw new InvalidOperationException(
                $"Не удалось получить пользователя для восстановления пароля. UserId: {userId}");
        }
        
        user.PasswordHash = passwordHash;
        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод обновляет логин пользователям.
    /// </summary>
    /// <param name="usersIds">Список Id пользователей.</param>
    public async Task UpdateUsersLoginAsync(IEnumerable<UserEntity> users)
    {
        _pgContext.Users.UpdateRange(users);
        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод актуализирует дату последней авторизации пользователя = сегодня.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    public async Task ActualisingLastAutorizationUserAsync(long userId)
    {
        var user = await _pgContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        user!.LastAutorization = DateTime.UtcNow;

        await _pgContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task SetSubscriptionAsync(int ruleId, long userId, int month, int days)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        using var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
        
        var startDate = DateTime.UtcNow; // Дата начала подписки.
        var endDate = startDate.AddDays(days); // Вычисляем дату окончания подписки.

        try
        {
            // Устанавливаем срок действия подписки пользователя.
            var setDatesParameters = new DynamicParameters();
            setDatesParameters.Add("@userId", userId);
            setDatesParameters.Add("@subscriptionStartDate", startDate);
            setDatesParameters.Add("@subscriptionEndDate", endDate);

            var setDatesQuery = "UPDATE dbo.\"Users\" " +
                                "SET \"SubscriptionStartDate\" = @subscriptionStartDate, " +
                                "\"SubscriptionEndDate\" = @subscriptionEndDate " +
                                "WHERE \"UserId\" = @userId";

            await connection.ExecuteAsync(setDatesQuery, setDatesParameters);

            var parameters = new DynamicParameters();
            parameters.Add("@ruleId", ruleId);
            parameters.Add("@userId", userId);
            parameters.Add("@month", month);

            var query = "UPDATE subscriptions.user_subscriptions " +
                        "SET month_count = @month, " +
                        "subscription_id = @ruleId " +
                        "WHERE user_id = @userId";
        
            await connection.ExecuteAsync(query, parameters);
        
            transaction.Commit();
        }
        
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
    
    /// <summary>
    /// Метод получает ФИО авторов задач по их Id.
    /// </summary>
    /// <param name="authorIds">Id авторов задач.</param>
    /// <returns>Словарь с авторами задач.</returns>
    public async Task<IDictionary<long, UserInfoOutput>> GetAuthorNamesByAuthorIdsAsync(IEnumerable<long> authorIds)
    {
		using var connection = await ConnectionProvider.GetConnectionAsync();

		var parameters = new DynamicParameters();
		parameters.Add("@userId", authorIds);

		var query =
			"SELECT Users.\"UserId\", Users.\"Email\" , Profile.\"FirstName\" , Profile.\"LastName\" " +
			"FROM dbo.\"Users\" as Users " +
			"INNER JOIN \"Profile\".\"ProfilesInfo\" as Profile " +
			"ON Users.\"UserId\" = Profile.\"UserId\" " +
			"WHERE Users.\"UserId\" = ANY (@userId) ";

		var result = (await connection.QueryAsync<UserInfoOutput>(query, parameters)).ToDictionary(k => k.UserId, v => v);

		return result;
	}

    /// <summary>
    /// Метод получает ФИО исполнителей задач по их Id.
    /// </summary>
    /// <param name="executorIds">Id исполнителей задач.</param>
    /// <returns>Словарь с исполнителями задач.</returns>
    public async Task<IDictionary<long, UserInfoOutput>> GetExecutorNamesByExecutorIdsAsync(
        IEnumerable<long> executorIds)
    {

		using var connection = await ConnectionProvider.GetConnectionAsync();

		var parameters = new DynamicParameters();
		parameters.Add("@userId", executorIds);

		var query = 
            "SELECT Users.\"UserId\", Users.\"Email\" , Profile.\"FirstName\" , Profile.\"LastName\" " +
			"FROM dbo.\"Users\" as Users " +
			"INNER JOIN \"Profile\".\"ProfilesInfo\" as Profile " +
			"ON Users.\"UserId\" = Profile.\"UserId\" " +
			"WHERE Users.\"UserId\" = ANY (@userId) ";

		var result = (await connection.QueryAsync<UserInfoOutput>(query, parameters)).ToDictionary(k => k.UserId, v => v);

		return result;
    }

    /// <summary>
    /// Метод получает ФИО наблюдателей задач по их Id.
    /// </summary>
    /// <param name="watcherIds">Id наблюдателей задач.</param>
    /// <returns>Словарь с наблюдателями задач.</returns>
    public async Task<IDictionary<long, UserInfoOutput>?> GetWatcherNamesByWatcherIdsAsync(IEnumerable<long> watcherIds)
    {
        var result = await _pgContext.Users
            .Where(u => watcherIds.Contains(u.UserId))
            .Select(u => new UserInfoOutput
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                SecondName = u.SecondName,
                UserId = u.UserId
            })
            .ToDictionaryAsync(k => k.UserId, v => v);

        return result;
    }

    /// <summary>
    /// Метод получает данные профиля пользователей по их Id.
    /// </summary>
    /// <param name="userIds">Id пользователей.</param>
    /// <returns>Данные профиля пользователей.</returns>
    public async Task<IEnumerable<ProfileInfoEntity>> GetProfileInfoByUserIdsAsync(IEnumerable<long> userIds)
    {
        var result = await _pgContext.ProfilesInfo
            .Where(p => userIds.Contains(p.UserId))
            .Select(p => new ProfileInfoEntity
            {
                UserId = p.UserId,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Patronymic = p.Patronymic
            })
            .ToListAsync();

        return result;
    }

    /// <inheritdoc />
    public async Task<Guid> GetUserCodeByUserIdAsync(long userId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId);

        var query = "SELECT \"UserCode\" " +
                    "FROM dbo.\"Users\" " +
                    "WHERE \"UserId\" = @userId";

        var result = await connection.QueryFirstOrDefaultAsync<Guid>(query, parameters);

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ComponentRoleOutput>> GetComponentRolesAsync()
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var query = "SELECT component_role_id, " +
                    "component_role_name, " +
                    "component_role_sys_name " +
                    "FROM roles.component_roles " +
                    "ORDER BY position";

        var result = (await connection.QueryAsync<ComponentRoleOutput>(query)).Select(x =>
        {
            x.ComponentRoleSysName = x.ComponentRoleSysName!.ToPascalCaseFromSnakeCase();

            return x;
        });

        return result;
    }

    /// <inheritdoc />
    public async Task AddComponentUserRolesAsync(long userId, IEnumerable<int>? componentRoles)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new List<DynamicParameters>();
        var compRoles = componentRoles.AsList();

        foreach (var cr in compRoles)
        {
            var tempParameters = new DynamicParameters();
            tempParameters.Add("@userId", userId);
            tempParameters.Add("@componentRoles", cr);
            
            parameters.Add(tempParameters);
        }

        var query = "INSERT INTO roles.component_user_roles (component_role_id, user_id) " +
                    "VALUES (@componentRoles, @userId)";

        await connection.ExecuteAsync(query, parameters);
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}