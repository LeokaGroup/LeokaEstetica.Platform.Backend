using AutoMapper;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Database.Abstractions.Vacancy;
using LeokaEstetica.Platform.Database.Chat;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Messaging.Abstractions.Chat;
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
                throw new NullReferenceException($"Id пользователя с аккаунтом {account} не найден!");
            }

            if (discussionTypeId > 0)
            {
                var ownerId = await GetOwnerIdAsync(discussionType, discussionTypeId);

                // Выбираем Id диалога с владельцем проекта.
                var ownerDialogId = await _chatRepository.GetDialogByUserIdAsync(ownerId);

                // Выбираем Id диалога с текущем пользователем.
                var currentDialogId = await _chatRepository.GetDialogByUserIdAsync(userId);

                // Если диалоги не нашли, то будем искать иначе.
                if (currentDialogId == 0 && ownerDialogId == 0)
                {
                    isFindDialog = false;
                }

                // Найдем диалог, в котором есть оба участника.
                var findDialogId = await _chatRepository.GetDialogMembersAsync(userId, ownerId);

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
                    var lastDialogId = await _chatRepository.CreateDialogAsync(string.Empty, DateTime.Now);

                    // Добавляем участников нового диалога.
                    await _chatRepository.AddDialogMembersAsync(userId, ownerId, lastDialogId);
                    result.DialogState = DialogStateEnum.Open.ToString();
                }

                // Если просматривает владелец объект обсуждения,
                // то собеседника нет и надо вернуть пустой диалог без его создания.
                if (currentDialogId == 0 
                    && ownerDialogId == 0
                    && !isFindDialog)
                {
                    result.DialogState = DialogStateEnum.Empty.ToString();

                    return result;
                }

                dialogId = ownerDialogId;
            }

            var convertDialogId = Convert.ToInt64(dialogId);

            // Проверяем существование диалога.
            var checkDialog = await _chatRepository.CheckDialogAsync(convertDialogId);

            if (!checkDialog)
            {
                throw new NullReferenceException($"Такого диалога не найдено. DialogId был {convertDialogId}");
            }

            // Получаем список сообщений диалога.
            var getMessages = await _chatRepository.GetDialogMessagesAsync(convertDialogId);
            
            // Получаем дату начала диалога.
            result.DateStartDialog = await _chatRepository.GetDialogStartDateAsync(convertDialogId);
            
            var user = await _userRepository.GetUserByUserIdAsync(userId);

            // Записываем полное ФИО пользователя, с которым идет общение в чате.
            result.FirstName = user.FirstName;
            result.LastName = user.LastName;
            result.FullName = result.FirstName + "" + result.LastName;

            // Если у диалога нет сообщений, значит вернуть пустой диалог, который будет открыт.
            if (!getMessages.Any())
            {
                result.DialogState = DialogStateEnum.Empty.ToString();

                return result;
            }

            var messages = _mapper.Map<List<DialogMessageOutput>>(getMessages);

            // Форматируем дату сообщений.
            foreach (var item in messages)
            {
                item.Created = string.Format("{0:f}", item.Created);
                
                // Помечаем сообщения текущего пользователя.
                item.IsMyMessage = item.UserId == userId;
            }

            result.DialogState = DialogStateEnum.Open.ToString();

            // Получаем список Id участников диалога.
            var memberIds = await _chatRepository.GetDialogMembersAsync(convertDialogId);

            if (!memberIds.Any())
            {
                throw new NullReferenceException($"Не найдено участников для диалога с DialogId {convertDialogId}");
            }
            
            result.Messages = messages;

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
            var result = new List<DialogOutput>();
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId == 0)
            {
                throw new NullReferenceException($"Id пользователя с аккаунтом {account} не найден!");
            }

            var dialogs = await _chatRepository.GetDialogsAsync(userId);

            if (!dialogs.Any())
            {
                return Enumerable.Empty<DialogOutput>();
            }
            
            foreach (var dialog in dialogs)
            {
                // Подтягиваем последнее сообщение для каждого диалога и проставляет после 40 символов ...
                var lastMessage = await _chatRepository.GetLastMessageAsync(dialog.DialogId);
                dialog.LastMessage = lastMessage.Length > 40
                    ? string.Concat(lastMessage.Substring(0, 40), "...")
                    : lastMessage;
                
                // Найдет Id участников диалога по DialogId.
                var membersIds = await _chatRepository.GetDialogMembersAsync(dialog.DialogId);
                
                if (membersIds == null)
                {
                    throw new NullReferenceException($"Не найдено участников для диалога с DialogId {dialog.DialogId}");
                }
                
                // Записываем имя и фамилию участника диалога, с которым идет общение.
                var otherUserId = membersIds.FirstOrDefault(m => !m.Equals(userId));
                var otherData = await _userRepository.GetUserByUserIdAsync(otherUserId);
                dialog.FullName = otherData.FirstName + " " + otherData.LastName;
                
                // Если дата диалога совпадает с сегодняшней, то заполнит часы и минуты, иначе оставит их null.
                if (DateTime.Now.ToString("d").Equals(Convert.ToDateTime(dialog.Created).ToString("d")))
                {
                    // Запишет только часы и минуты.
                    dialog.CalcTime = Convert.ToDateTime(dialog.Created).ToString("t");
                }

                // Если дата диалога не совпадает с сегодняшней.
                else
                {
                    // Записываем только дату.
                    dialog.CalcShortDate = Convert.ToDateTime(dialog.Created).ToString("d");
                }
                
                // Форматируем дату убрав секунды.
                dialog.Created = Convert.ToDateTime(dialog.Created).ToString("g");
            }
            
            result.AddRange(dialogs);
            
            return result;
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }
}