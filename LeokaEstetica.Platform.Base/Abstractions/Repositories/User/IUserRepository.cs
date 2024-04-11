using LeokaEstetica.Platform.Models.Dto.Output.User;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.Profile;
using LeokaEstetica.Platform.Models.Entities.Ticket;
using LeokaEstetica.Platform.Models.Entities.User;

namespace LeokaEstetica.Platform.Base.Abstractions.Repositories.User;

/// <summary>
/// Абстракция репозитория пользователей.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Метод сохраняет нового пользователя в базу.
    /// </summary>
    /// <param name="user">Данные пользователя для добавления.</param>
    /// <returns>Id пользователя.</returns>
    Task<long> AddUserAsync(UserEntity user); 

    /// <summary>
    /// Метод находит пользователя по его UserId.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Данные пользователя.</returns>
    Task<UserEntity> GetUserByUserIdAsync(long userId);

    /// <summary>
    /// Метод проверет существование пользователя по email в базе.
    /// </summary>
    /// <param name="email">Email пользователя.</param>
    /// <returns>Флаг проверки.</returns>
    Task<bool> CheckUserByEmailAsync(string email);

    /// <summary>
    /// Метод запишет код подтверждения пользователю.
    /// </summary>
    /// <param name="code">Код подтверждения, который мы отправили пользователю на почту.</param>
    /// <param name="userId">UserId.</param>
    Task SetConfirmAccountCodeAsync(Guid code, long userId);
    
    /// <summary>
    /// Метод подтверждает аккаунт пользователя по коду, который ранее был отправлен пользователю на почту и записан в БД.
    /// </summary>
    /// <param name="code">Код подтверждения.</param>
    /// <returns>Статус подтверждения.</returns>
    Task<bool> ConfirmAccountAsync(Guid code);

    /// <summary>
    /// Метод получает хэш пароля для проверки пользователя.
    /// </summary>
    /// <param name="email">Почта.</param>
    /// <returns>Хэш пароля.</returns>
    Task<string> GetPasswordHashByEmailAsync(string email);

    /// <summary>
    /// Метод находит Id пользователя по его почте.
    /// </summary>
    /// <param name="account">Почта пользователя.</param>
    /// <returns>Id пользователя.</returns>
    Task<long> GetUserByEmailAsync(string account);

    /// <summary>
    /// Метод получает код пользователя по его почте.
    /// </summary>
    /// <param name="email">Почта.</param>
    /// <returns>Хэш пароля.</returns>
    Task<Guid> GetUserCodeByEmailAsync(string email);
    
    /// <summary>
    /// Метод получает код пользователя по его VkUserId.
    /// </summary>
    /// <param name="vkUserId">VkUserId пользователя.</param>
    /// <returns>Хэш пароля.</returns>
    Task<Guid> GetUserCodeByVkUserIdAsync(long vkUserId);

    /// <summary>
    /// Метод получает недостающую информацию профиля по UserId.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Основные данные профиля.</returns>
    Task<UserInfoOutput> GetUserPhoneEmailByUserIdAsync(long userId);

    /// <summary>
    /// Метод сохраняет данные для таблицы пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="phone">Номер телефона.</param>
    /// <param name="email">Email.</param>
    Task<SaveUserProfileDataOutput> SaveUserDataAsync(long userId, string phone, string email);

    /// <summary>
    /// Метод находит логин и почту пользователей по почте или логину пользователя.
    /// </summary>
    /// <param name="searchText">Текст, по которому надо искать.</param>
    /// <returns>Список пользователей.</returns>
    Task<List<UserEntity>> GetUserByEmailOrLoginAsync(string searchText);
    
    /// <summary>
    /// Метод находит Id пользователя по почте или логину пользователя.
    /// </summary>
    /// <param name="searchText">Текст, по которому надо искать.</param>
    /// <returns>Id пользователя.</returns>
    Task<long> GetUserIdByEmailOrLoginAsync(string searchText);

    /// <summary>
    /// Метод получает словарь кодов пользователей.
    /// </summary>
    /// <returns>Словарь кодов пользователей.</returns>
    Task<Dictionary<long, Guid>> GetUsersCodesAsync();
    
    /// <summary>
    /// Метод получает словарь кодов пользователей, Id которых передали.
    /// </summary>
    /// <returns>Словарь кодов пользователей.</returns>
    Task<Dictionary<long, Guid>> GetUsersCodesByUserIdsAsync(IEnumerable<long> userIds);

    /// <summary>
    /// Метод получает список пользователей.
    /// </summary>
    /// <returns>Список пользователей.</returns>
    Task<List<UserEntity>> GetAllAsync();

    /// <summary>
    /// Метод находит Id пользователя по его коду.
    /// </summary>
    /// <param name="userCode">Код пользователя.</param>
    /// <returns>Id пользователя.</returns>
    Task<long> GetUserIdByCodeAsync(Guid userCode);
    
    /// <summary>
    /// Метод находит Id пользователя по его Email.
    /// </summary>
    /// <param name="email">Email пользователя.</param>
    /// <returns>Id пользователя.</returns>
    Task<long> GetUserIdByEmailAsync(string email);
    
    /// <summary>
    /// Метод находит Id пользователя по его номеру телефона.
    /// </summary>
    /// <param name="phoneNumber">Номер телефона пользователя.</param>
    /// <returns>Id пользователя.</returns>
    Task<long> GetUserIdByPhoneNumberAsync(string phoneNumber);
    
    /// <summary>
    /// Метод находит Id пользователя по его логину.
    /// </summary>
    /// <param name="phoneNumber">Логин пользователя.</param>
    /// <returns>Id пользователя.</returns>
    Task<long> GetUserIdByLoginAsync(string login);

    /// <summary>
    /// Метод проверет существование пользователя по VkUserId в базе.
    /// </summary>
    /// <param name="userId">VkUserId пользователя.</param>
    /// <returns>Флаг проверки.</returns>
    Task<bool> CheckUserByVkUserIdAsync(long userId);

    /// <summary>
    /// Метод находит Id пользователя по его vk id.
    /// </summary>
    /// <param name="vkId">Id вконтакте.</param>
    /// <returns>Id пользователя.</returns>
    Task<long> GetUserIdByVkIdAsync(long vkId);

    /// Метод проставляет пользователям метку к удалению аккаунтов.
    /// </summary>
    /// <param name="users">Список пользователей, которых предупредим.</param>
    Task SetMarkDeactivateAccountsAsync(List<UserEntity> users);
    
    /// <summary>
    /// Метод удаляет аккаунты пользователей.
    /// </summary>
    /// <param name="users">Список пользователей, которых удаляем.</param>
    /// <param name="profileItems">Список анкет пользователей, которых удаляем.</param>
    /// <param name="profileItems">Список анкет пользователей на модерации, которых удаляем.</param>
    /// <param name="ticketsMembersItems">Список участников тикетов, которые удаляем.</param>
    /// <param name="ticketsMessagesItems">Список сообщений тикетов, которые удаляем.</param>
    Task DeleteDeactivateAccountsAsync(List<UserEntity> users, List<ProfileInfoEntity> profileItems,
        List<ModerationResumeEntity> moderationResumes, List<TicketMemberEntity> ticketsMembersItems,
        List<TicketMessageEntity> ticketsMessagesItems);
    
    /// <summary>
    /// Метод находит Id пользователя по его Id анкеты.
    /// </summary>
    /// <param name="profileInfoId">Id анкеты.</param>
    /// <returns>Id пользователя.</returns>
    Task<long> GetUserIdByProfileInfoIdAsync(long profileInfoId);
    
    /// <summary>
    /// Метод находит аккаунт пользователя по его Id анкеты.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Аккаунт пользователя.</returns>
    Task<string> GetUserAccountByUserIdAsync(long userId);
    
    /// <summary>
    /// Метод находит Id анкеты по Id пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Id анкеты.</returns>
    Task<long> GetProfileInfoIdByUserIdAsync(long userId);

    /// <summary>
    /// Метод получает дату начала подписки и дату окончания подписки пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Дата начала и дата окончания подписки.</returns>
    Task<(DateTime? StartDate, DateTime? EndDate)> GetUserSubscriptionUsedDateAsync(long userId);

    /// <summary>
    /// Метод проставляет срок подписки пользователю.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="startDate">Дата начала подписки.</param>
    /// <param name="endDate">Дата конца подписки.</param>
    /// <returns>Признак записи подписки пользователю.</returns>
    Task<bool> SetSubscriptionDatesAsync(long userId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Метод восстанавливает пароль создавая новый.
    /// </summary>
    /// <param name="passwordHash">Хэш пароля.</param>
    /// <param name="userId">Id пользователя.</param>
    Task RestoreUserPasswordAsync(string passwordHash, long userId);

    /// <summary>
    /// Метод обновляет логин пользователям.
    /// </summary>
    /// <param name="users">Список пользователей.</param>
    Task UpdateUsersLoginAsync(IEnumerable<UserEntity> users);

    /// <summary>
    /// Метод актуализирует дату последней авторизации пользователя = сегодня.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    Task ActualisingLastAutorizationUserAsync(long userId);

    /// <summary>
    /// Метод проставляет подписку пользователю.
    /// </summary>
    /// <param name="ruleId">Id тарифа, на который переходит пользователь.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="month">Кол-во месяцев, на которое оформляется подписка.</param>
    Task SetSubscriptionAsync(int ruleId, long userId, int month);
    
    /// <summary>
    /// Метод получает ФИО авторов задач по их Id.
    /// </summary>
    /// <param name="authorIds">Id авторов задач.</param>
    /// <returns>Словарь с авторами задач.</returns>
    Task<IDictionary<long, UserInfoOutput>> GetAuthorNamesByAuthorIdsAsync(IEnumerable<long> authorIds);
    
    /// <summary>
    /// Метод получает ФИО исполнителей задач по их Id.
    /// </summary>
    /// <param name="executorIds">Id исполнителей задач.</param>
    /// <returns>Словарь с исполнителями задач.</returns>
    Task<IDictionary<long, UserInfoOutput>> GetExecutorNamesByExecutorIdsAsync(IEnumerable<long> executorIds);
    
    /// <summary>
    /// Метод получает ФИО наблюдателей задач по их Id.
    /// </summary>
    /// <param name="watcherIds">Id наблюдателей задач.</param>
    /// <returns>Словарь с наблюдателями задач.</returns>
    Task<IDictionary<long, UserInfoOutput>> GetWatcherNamesByWatcherIdsAsync(IEnumerable<long> watcherIds);
}