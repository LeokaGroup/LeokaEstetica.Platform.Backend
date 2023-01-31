using System.Globalization;
using AutoMapper;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Database.Abstractions.Vacancy;
using LeokaEstetica.Platform.Database.Chat;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Messaging.Abstractions.Chat;
using LeokaEstetica.Platform.Messaging.Builders;
using LeokaEstetica.Platform.Messaging.Enums;
using LeokaEstetica.Platform.Messaging.Models.Chat.Output;
using LeokaEstetica.Platform.Models.Dto.Chat.Output;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Messaging.Services.Chat;

/// <summary>
/// Класс реализует методы сервиса чата.
/// </summary>
public sealed class ChatService : IChatService
{
    private readonly ILogService _logService;
    private readonly IUserRepository _userRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IVacancyRepository _vacancyRepository;
    private readonly IChatRepository _chatRepository;
    private readonly IMapper _mapper;

    public ChatService(ILogService logService,
        IUserRepository userRepository,
        IProjectRepository projectRepository,
        IVacancyRepository vacancyRepository,
        IChatRepository chatRepository,
        IMapper mapper)
    {
        _logService = logService;
        _userRepository = userRepository;
        _projectRepository = projectRepository;
        _vacancyRepository = vacancyRepository;
        _chatRepository = chatRepository;
        _mapper = mapper;
    }

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
                throw new NullReferenceException($"Id пользователя с аккаунтом {account} не найден.");
            }

            if (discussionTypeId <= 0)
            {
                throw new NullReferenceException("Не передали Id предмета обсуждения.");
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
                dialogId = await _chatRepository.CreateDialogAsync(string.Empty, DateTime.Now);

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
                dialogId = await _chatRepository.CreateDialogAsync(string.Empty, DateTime.Now);

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

            dialogId ??= ownerDialogId; // Если DialogId null, то присваиваем ему ownerDialogId.

            var convertDialogId = (long)dialogId;

            // Проверяем существование диалога.
            var checkDialog = await _chatRepository.CheckDialogAsync(convertDialogId);

            if (!checkDialog)
            {
                throw new NullReferenceException($"Такого диалога не найдено. DialogId был {convertDialogId}");
            }

            // Получаем список Id участников диалога.
            var memberIds = await _chatRepository.GetDialogMembersAsync(convertDialogId);

            if (!memberIds.Any())
            {
                throw new NullReferenceException($"Не найдено участников для диалога с DialogId {convertDialogId}");
            }

            // Получаем список сообщений диалога.
            var getMessages = await _chatRepository.GetDialogMessagesAsync(convertDialogId);

            // Получаем дату начала диалога.
            result.DateStartDialog = await _chatRepository.GetDialogStartDateAsync(convertDialogId);

            var user = await _userRepository.GetUserByUserIdAsync(userId);

            // Записываем полное ФИО пользователя, с которым идет общение в чате.
            result.FirstName = user.FirstName;
            result.LastName = user.LastName;
            result.FullName = await CreateDialogOwnerFioAsync(userId);

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

            result.DialogState = DialogStateEnum.Open.ToString();

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

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
    /// Метод получает список диалогов.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список диалогов.</returns>
    public async Task<IEnumerable<DialogOutput>> GetDialogsAsync(string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId == 0)
            {
                throw new NullReferenceException($"Id пользователя с аккаунтом {account} не найден.");
            }

            var dialogs = await _chatRepository.GetDialogsAsync(userId);
            dialogs = await CreateDialogMessagesBuilder.Create(dialogs, _chatRepository, _userRepository, userId);

            return dialogs;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
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
            var result = new DialogResultOutput { Messages = new List<DialogMessageOutput>() };

            // Находим Id текущего пользователя, который просматривает страницу проекта или вакансии.
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId == 0)
            {
                throw new NullReferenceException($"Id пользователя с аккаунтом {account} не найден.");
            }

            if (discussionTypeId <= 0)
            {
                throw new NullReferenceException("Не передали Id предмета обсуждения.");
            }

            var ownerId = await GetOwnerIdAsync(discussionType, discussionTypeId);

            // Найдем диалог, в котором есть оба участника, отталкиваемся от текущего пользователя.
            var findDialogId = await _chatRepository.GetDialogMembersByUserIdAsync(userId);

            // Если диалог уже есть, ничего не делать.
            if (findDialogId > 0)
            {
                result.DialogState = DialogStateEnum.Empty.ToString();
                result.DialogId = findDialogId;
                result.FullName = await CreateDialogOwnerFioAsync(userId);

                return result;
            }

            // Проверяем существование диалога перед его созданием.
            var isDublicateDialog = await _chatRepository.CheckDialogAsync(userId, ownerId);

            // Диалога нет, можем создавать.
            if (!isDublicateDialog)
            {
                // Создаем новый диалог.
                var lastDialogId = await _chatRepository.CreateDialogAsync(string.Empty, DateTime.Now);

                // Добавляем участников нового диалога.
                await _chatRepository.AddDialogMembersAsync(userId, ownerId, lastDialogId);
                result.DialogState = DialogStateEnum.Open.ToString();
                result.DialogId = lastDialogId;

                // Получаем дату начала диалога.
                result.DateStartDialog = await _chatRepository.GetDialogStartDateAsync(result.DialogId);
            }

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод строит строку с имененм и фамилией пользователя, с которым идет общение.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Строка с именем и фамилией.</returns>
    private async Task<string> CreateDialogOwnerFioAsync(long userId)
    {
        var result = await _userRepository.GetUserByUserIdAsync(userId);

        return result.FirstName + " " + result.LastName;
    }

    /// <summary>
    /// Метод отправляет сообщение.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    /// <param name="dialogId">Id диалога.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Выходная модель.</returns>
    public async Task<DialogResultOutput> SendMessageAsync(string message, long dialogId, string account)
    {
        try
        {
            var result = new DialogResultOutput { Messages = new List<DialogMessageOutput>() };

            // Если нет сообщения, то ничего не делать.
            if (string.IsNullOrEmpty(message))
            {
                return null;
            }

            if (dialogId == 0)
            {
                throw new ArgumentException("Id диалога не может быть пустым.");
            }

            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId == 0)
            {
                throw new NullReferenceException($"Id пользователя с аккаунтом {account} не найден.");
            }

            // Проверяем существование диалога.
            var checkDialog = await _chatRepository.CheckDialogAsync(dialogId);

            if (!checkDialog)
            {
                throw new NullReferenceException($"Такого диалога не найдено. DialogId был {dialogId}");
            }

            // Записываем сообщение в БД.
            await _chatRepository.SaveMessageAsync(message, dialogId, DateTime.Now, userId, true);

            // Получаем список сообщений диалога.
            var messages = await _chatRepository.GetDialogMessagesAsync(dialogId);

            // Проставляем флаг принадлежности сообщений.
            foreach (var msg in messages)
            {
                msg.IsMyMessage = msg.UserId == userId;
            }

            result.DialogState = DialogStateEnum.Open.ToString();
            var mapMessages = _mapper.Map<List<DialogMessageOutput>>(messages);
            result.Messages.AddRange(mapMessages);

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }
}