using System.Globalization;
using System.Runtime.CompilerServices;
using AutoMapper;
using LeokaEstetica.Platform.Base.Abstractions.Messaging.Chat;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Chat;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Base.Builders;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.Vacancy;
using LeokaEstetica.Platform.Messaging.Builders;
using LeokaEstetica.Platform.Messaging.Enums;
using LeokaEstetica.Platform.Models.Dto.Chat.Output;
using LeokaEstetica.Platform.Models.Enums;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Messaging.Services.Chat;

/// <summary>
/// Класс реализует методы сервиса чата.
/// </summary>
internal sealed class ChatService : IChatService
{
    private readonly ILogger<ChatService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IVacancyRepository _vacancyRepository;
    private readonly IChatRepository _chatRepository;
    private readonly IMapper _mapper;
    private readonly IProjectResponseRepository _projectResponseRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="projectRepository">Репозиторий проектов.</param>
    /// <param name="vacancyRepository">Репозиторий вакансий.</param>
    /// <param name="chatRepository">Репозиторий чата.</param>
    /// <param name="mapper">Маппер.</param>
    /// <param name="projectResponseRepository">Репозиторий откликов на проекты.</param>
    public ChatService(ILogger<ChatService> logger,
        IUserRepository userRepository,
        IProjectRepository projectRepository,
        IVacancyRepository vacancyRepository,
        IChatRepository chatRepository,
        IMapper mapper,
        IProjectResponseRepository projectResponseRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _projectRepository = projectRepository;
        _vacancyRepository = vacancyRepository;
        _chatRepository = chatRepository;
        _mapper = mapper;
        _projectResponseRepository = projectResponseRepository;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод получает диалог или создает новый и возвращает его.
    /// </summary>
    /// <param name="dialogId">Id диалога.</param>
    /// <param name="discussionType">Тип объекта обсуждения.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="discussionTypeId">Id предмета обсуждения (Id проекта или вакансии).</param>
    /// <returns>Данные диалога.</returns>
    public async Task<DialogResultOutput> GetDialogAsync(long? dialogId, DiscussionTypeEnum discussionType,
        string account, long discussionTypeId)
    {
        try
        {
            var result = new DialogResultOutput { Messages = new List<DialogMessageOutput>() };
            var isFindDialog = false;

            // Находим Id текущего пользователя, который просматривает страницу проекта или вакансии.
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId == 0)
            {
                throw new InvalidOperationException($"Id пользователя с аккаунтом {account} не найден.");
            }

            // Если не передали Id предмета обсуждения, то если проект,
            // то пойдем искать Id проекта у диалога, так как они связаны.
            if (discussionTypeId <= 0 
                && discussionType == DiscussionTypeEnum.Project 
                && dialogId.HasValue)
            {
                discussionTypeId = await _chatRepository.GetDialogProjectIdByDialogIdAsync(dialogId.Value);
            }

            // Если снова не нашли, то это уже ошибка.
            if (discussionTypeId <= 0)
            {
                throw new InvalidOperationException("Не передали Id предмета обсуждения.");
            }

            var ownerId = await GetOwnerIdAsync(discussionType, discussionTypeId);

            // Выбираем Id диалога с владельцем проекта.
            var ownerDialogId = await _chatRepository.GetDialogByUserIdAsync(ownerId);

            // Выбираем Id диалога с текущем пользователем.
            var currentDialogId = await _chatRepository.GetDialogByUserIdAsync(userId);

            // Найдем диалог, в котором есть оба участника, отталкиваемся от текущего пользователя.
            var findDialogId = await _chatRepository.GetDialogMembersByUserIdAsync(userId);

            if (findDialogId == 0)
            {
                // Создаем новый диалог.
                dialogId = await _chatRepository.CreateDialogAsync(string.Empty, DateTime.UtcNow);

                // Добавляем участников нового диалога.
                await _chatRepository.AddDialogMembersAsync(userId, ownerId, (long)dialogId);
                result.DialogState = DialogStateEnum.Open.ToString();
                result.DialogId = (long)dialogId;

                return result;
            }

            if (findDialogId > 0)
            {
                isFindDialog = true;
                ownerDialogId = findDialogId;
            }

            // Сравниваем DialogId текущего пользователя с владельцем проекта.
            // Если они равны, значит текущий пользователь общается с владельцем.
            if (currentDialogId != ownerDialogId
                && currentDialogId > 0
                && ownerDialogId > 0
                && !isFindDialog)
            {
                // Создаем новый диалог.
                dialogId = await _chatRepository.CreateDialogAsync(string.Empty, DateTime.UtcNow);

                // Добавляем участников нового диалога.
                await _chatRepository.AddDialogMembersAsync(userId, ownerId, (long)dialogId);
                result.DialogState = DialogStateEnum.Open.ToString();

                return result;
            }

            // Если просматривает владелец объект обсуждения,
            // то собеседника нет и надо вернуть пустой диалог без его создания.
            if (currentDialogId == 0
                && ownerDialogId == 0
                && !isFindDialog)
            {
                result.DialogState = DialogStateEnum.Empty.ToString();
                result.FullName = await CreateDialogOwnerFioAsync(userId);

                return result;
            }

            dialogId ??= ownerDialogId; // Если dialogId == null, то берем ownerDialogId.

            var convertDialogId = (long)dialogId;

            // Проверяем существование диалога.
            var checkDialog = await _chatRepository.CheckDialogAsync(convertDialogId);

            if (!checkDialog)
            {
                throw new InvalidOperationException($"Такого диалога не найдено. DialogId был {convertDialogId}");
            }

            // Получаем список Id участников диалога.
            var memberIds = await _chatRepository.GetDialogMembersAsync(convertDialogId);

            if (!memberIds.Any())
            {
                throw new InvalidOperationException($"Не найдено участников для диалога с DialogId {convertDialogId}");
            }

            // Получаем список сообщений диалога.
            var getMessages = await _chatRepository.GetDialogMessagesAsync(convertDialogId);
            var user = await _userRepository.GetUserByUserIdAsync(userId);

            // Записываем полное ФИО пользователя, с которым идет общение в чате.
            if (user.FirstName is not null && user.LastName is not null)
            {
                result.FirstName = user.FirstName;
                result.LastName = user.LastName;   
            }

            // Исключаем текущего пользователя.
            var newMembers = memberIds.Distinct();
            long id;

            // Если там было 2 дубля пользователя, то выберем просто текущего.
            if (newMembers.Count() <= 1)
            {
                id = userId;
            }

            // Иначе берем первого исключая текущего.
            else
            {
                id = memberIds.Except(new[] { userId }).First();
            }
            
            result.FullName = await CreateDialogOwnerFioAsync(id);
            result.DialogId = convertDialogId;
            
            // Получаем дату начала диалога.
            result.DateStartDialog = await _chatRepository.GetDialogStartDateAsync(convertDialogId);
            result.DialogState = DialogStateEnum.Open.ToString();

            // Если у диалога нет сообщений, значит вернуть пустой диалог, который будет открыт.
            if (!getMessages.Any())
            {
                result.DialogState = DialogStateEnum.Empty.ToString();

                return result;
            }

            foreach (var item in getMessages)
            {
                var msg = _mapper.Map<DialogMessageOutput>(item);

                // Помечаем сообщения текущего пользователя.
                msg.IsMyMessage = item.UserId == userId;

                // Форматируем дату сообщения.
                msg.Created = item.Created.ToString("g", CultureInfo.GetCultureInfo("ru"));

                result.Messages.Add(msg);
            }

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список диалогов.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <param name="projectId">Id проекта. Если не передан, то получает все диалоги пользователя.</param>
    /// <returns>Список диалогов.</returns>
    public async Task<IEnumerable<DialogOutput>> GetDialogsAsync(string account, long? projectId = null)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId == 0)
            {
                throw new InvalidOperationException($"Id пользователя с аккаунтом {account} не найден.");
            }

            var dialogs = await _chatRepository.GetDialogsAsync(userId, projectId);
            var mapDialogs = _mapper.Map<List<ProfileDialogOutput>>(dialogs);
            
            dialogs = await CreateDialogMessagesBuilder.CreateDialogAsync((dialogs, mapDialogs), _chatRepository,
                _userRepository, userId, _mapper, account);

            return dialogs;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод создает диалог для написания владельцу проекта.
    /// Если такой диалог уже создан с текущим юзером и владельцем проекта,
    /// то ничего не происходит и диалог считается пустым для начала общения.
    /// <param name="discussionType">Тип объекта обсуждения.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="discussionTypeId">Id предмета обсуждения (Id проекта или вакансии).</param>
    /// <returns>Данные диалога.</returns>
    public async Task<DialogResultOutput> WriteProjectDialogOwnerAsync(DiscussionTypeEnum discussionType,
        string account, long discussionTypeId)
    {
        try
        {
            // Находим Id текущего пользователя, который просматривает страницу проекта или вакансии.
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId == 0)
            {
                throw new InvalidOperationException($"Id пользователя с аккаунтом {account} не найден.");
            }

            if (discussionTypeId <= 0)
            {
                throw new InvalidOperationException("Не передали Id предмета обсуждения.");
            }

            // Найдем диалог, в котором есть оба участника, отталкиваемся от текущего пользователя.
            var findDialogId = await _chatRepository.GetDialogMembersByUserIdAsync(userId);
            
            var result = new DialogResultOutput { Messages = new List<DialogMessageOutput>() };

            // Если диалог уже есть, ничего не делать.
            if (findDialogId > 0)
            {
                result.DialogState = DialogStateEnum.Empty.ToString();
                result.DialogId = findDialogId;
                result.FullName = await CreateDialogOwnerFioAsync(userId);
                
                // Устанавливаем связь, если диалог создается для проекта.
                if (discussionType == DiscussionTypeEnum.Project)
                {
                    // Связываем диалог с проектом.
                    await _chatRepository.SetReferenceProjectDialogAsync(result.DialogId, discussionTypeId);
                    result.ProjectId = discussionTypeId;
                }

                return result;
            }
            
            var ownerId = await GetOwnerIdAsync(discussionType, discussionTypeId);

            // Проверяем существование диалога перед его созданием.
            var isDublicateDialog = await _chatRepository.CheckDialogAsync(userId, ownerId);

            // Диалога нет, можем создавать.
            if (isDublicateDialog is null)
            {
                // Создаем новый диалог.
                var lastDialogId = await _chatRepository.CreateDialogAsync(string.Empty, DateTime.UtcNow);

                // Добавляем участников нового диалога.
                await _chatRepository.AddDialogMembersAsync(userId, ownerId, lastDialogId);
                result.DialogState = DialogStateEnum.Open.ToString();
                result.DialogId = lastDialogId;

                var dialogId = result.DialogId;

                // Получаем дату начала диалога.
                result.DateStartDialog = await _chatRepository.GetDialogStartDateAsync(dialogId);
                
                // Устанавливаем связь, если диалог создается для проекта.
                if (discussionType == DiscussionTypeEnum.Project)
                {
                    // Связываем диалог с проектом.
                    await _chatRepository.SetReferenceProjectDialogAsync(dialogId, discussionTypeId);
                    result.ProjectId = discussionTypeId;
                    
                    // Связываем диалог с вакансией (если при отклике на проект, отклик был с указанием вакансии).
                    await _projectResponseRepository.SetReferenceVacancyDialogAsync(discussionTypeId, userId);
                }
                
                return result;
            }
            
            result.DialogState = DialogStateEnum.Empty.ToString();
            result.DialogId = (long)isDublicateDialog;
            result.FullName = await CreateDialogOwnerFioAsync(ownerId);

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод отправляет сообщение.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    /// <param name="dialogId">Id диалога.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Выходная модель.</returns>
    public async Task<DialogResultOutput> SendMessageAsync(string message, long dialogId, long userId, string token)
    {
        try
        {
            // Проверяем существование диалога.
            var checkDialog = await _chatRepository.CheckDialogAsync(dialogId);

            if (!checkDialog)
            {
                throw new InvalidOperationException($"Такого диалога не найдено. DialogId был {dialogId}");
            }

            // Записываем сообщение в БД.
            await _chatRepository.SaveMessageAsync(message, dialogId, DateTime.UtcNow, userId, true);

            // Получаем список сообщений диалога.
            var messages = await _chatRepository.GetDialogMessagesAsync(dialogId);

            var result = new DialogResultOutput
            {
                Messages = new List<DialogMessageOutput>(),
                DialogState = DialogStateEnum.Open.ToString()
            };

            var mapMessages = _mapper.Map<List<DialogMessageOutput>>(messages);
            result.Messages.AddRange(mapMessages);
            
            // Получаем коды пользователей.
            var userIds = result.Messages.Select(u => u.UserId).Distinct();
            var userCodes = await _userRepository.GetUsersCodesByUserIdsAsync(userIds);
            
            // Проставляем флаг принадлежности сообщений.
            foreach (var msg in result.Messages)
            {
                msg.IsMyMessage = msg.UserId == userId;
                
                if (userCodes.ContainsKey(msg.UserId))
                {
                    // Записываем код пользователя.
                    msg.UserCode = userCodes.TryGet(msg.UserId);   
                }
            }

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получит все диалоги для профиля пользователя.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список диалогов.</returns>
    public async Task<List<ProfileDialogOutput>> GetProfileDialogsAsync(string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId == 0)
            {
                throw new InvalidOperationException($"Id пользователя с аккаунтом {account} не найден.");
            }

            var dialogs = await _chatRepository.GetProfileDialogsAsync(userId);
            var mapProfileDialogs = _mapper.Map<List<ProfileDialogOutput>>(dialogs);
            var mapDefaultDialogs =  _mapper.Map<List<DialogOutput>>(dialogs);
            
            dialogs = await CreateDialogMessagesBuilder.CreateProfileDialogAsync((mapDefaultDialogs, mapProfileDialogs),
                _chatRepository, _userRepository, userId, _mapper, account);

            return dialogs;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод записывает Id владельца предмета обсуждения.
    /// </summary>
    /// <param name="discussionType">Тип предмета обсуждения.</param>
    /// <param name="discussionTypeId">Id предмета обсуждения (вакансии, проекта и тд).</param>
    /// <returns></returns>
    private async Task<long> GetOwnerIdAsync(DiscussionTypeEnum discussionType, long discussionTypeId)
    {
        long ownerId = 0;

        // Если предмет обсуждения это проект.
        if (discussionType == DiscussionTypeEnum.Project)
        {
            // Выбираем Id владельца проекта.
            ownerId = await _projectRepository.GetProjectOwnerIdAsync(discussionTypeId);
        }

        // Если предмет обсуждения это вакансия.
        if (discussionType == DiscussionTypeEnum.Vacancy)
        {
            // Выбираем Id владельца вакансии.
            ownerId = await _vacancyRepository.GetVacancyOwnerIdAsync(discussionTypeId);
        }

        return ownerId;
    }
    
    /// <summary>
    /// Метод строит строку с имененм и фамилией пользователя, с которым идет общение.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Строка с именем и фамилией.</returns>
    private async Task<string> CreateDialogOwnerFioAsync(long userId)
    {
        var result = await _userRepository.GetUserByUserIdAsync(userId);

        if (result.FirstName is null 
            && result.LastName is null)
        {
            return result.Email;
        }

        return result.FirstName + " " + result.LastName;
    }

    #endregion
}