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
    /// Id нейросети Scrum Master AI.
    /// </summary>
    private const short SCRUM_MASTER_AI_ID = -1;

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

    /// <inheritdoc />
    public async Task<DialogResultOutput> GetDialogAsync(long? dialogId, DiscussionTypeEnum discussionType,
        string account, long discussionTypeId, bool isManualNewDialog, string? token)
    {
        try
        {
            // Находим Id текущего пользователя, который просматривает страницу проекта или вакансии.
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId == 0)
            {
                throw new InvalidOperationException($"Id пользователя с аккаунтом {account} не найден.");
            }
            
            var result = new DialogResultOutput { Messages = new List<DialogMessageOutput>() };

            // Этот диалог создается вручную - по кнопке и тд, принудительно создаем диалог и возвраащем его.
            // TODO: Добавить удаление пустого диалога с шаблонным текстом нейросети, если пользователь
            // TODO: не писал в него сутки. Чтобы не плодить пустые диалоги.
            if (isManualNewDialog && discussionType == DiscussionTypeEnum.ObjectTypeDialogAi)
            {
                // Создаем новый диалог.
                dialogId = await _chatRepository.CreateDialogAsync(string.Empty, DateTime.UtcNow, true);

                if (!dialogId.HasValue)
                {
                    throw new InvalidOperationException("Не удалось создать диалог либо у него битые данные.");
                }

                // Добавляем участников нового диалога.
                // -1 - это Id нейросети, добавляется автоматически в участники диалога.
                // TODO: Если в будущем будет несколько нейросетей, то у них будут разные Id, но отрицательные.
                // TODO: Тогда получать такие Id будем уже с БД, а пока хардкодим -1.
                await _chatRepository.AddDialogMembersAsync(userId, SCRUM_MASTER_AI_ID, dialogId.Value, true);

                if (string.IsNullOrWhiteSpace(token))
                {
                    throw new InvalidOperationException("Не передан токен пользователя для работы сокетов.");
                }
                
                // Генерим автоматическое сообщение от системы, потому что для отображения диалога на фронте,
                // нужно минимум 1 сообщение.
                await SendMessageAsync("Задайте мне первый свой вопрос. Я готова Вам помочь.", dialogId.Value,
                        SCRUM_MASTER_AI_ID, token, false, true)
                    .ConfigureAwait(false);
                
                result.DialogState = DialogStateEnum.Open.ToString();
                result.DialogId = dialogId.Value;

                return result;
            }

            // Если не передали Id предмета обсуждения, то если проект,
            // то пойдем искать Id проекта у диалога, так как они связаны.
            // TODO: В будущем возможно тут надо проверять еще на != DiscussionTypeEnum.ScrumMasterAi.
            if (discussionTypeId <= 0 
                && discussionType == DiscussionTypeEnum.Project 
                && dialogId.HasValue)
            {
                discussionTypeId = await _chatRepository.GetDialogProjectIdByDialogIdAsync(dialogId.Value);
            }
            
            // TODO: Когда нейросеть научиться работать учитывая предмет обсуждения (проект, вакансию).
            // TODO: Пока что она умет просто на общие вопросы отвечать без учета предмета обсуждения.
            // Если чат нейросети.
            // if (discussionTypeId <= 0 
            //     && discussionType == DiscussionTypeEnum.ObjectTypeDialogAi 
            //     && dialogId.HasValue)
            // {
            //     discussionTypeId = await _chatRepository.GetDialogProjectIdByDialogIdAsync(dialogId.Value);
            // }

            // TODO: Вторую проверку убрать, когда научим нейросеть учитывать предмет обсуждения (логика выше).
            // Если снова не нашли, то это уже ошибка.
            if (discussionTypeId <= 0 && discussionType != DiscussionTypeEnum.ObjectTypeDialogAi)
            {
                throw new InvalidOperationException("Не передали Id предмета обсуждения.");
            }

            // TODO: -1 это Id нейросети. Если станет больше нейросетей, то из БД получать будем такие Id.
            var ownerId = discussionType != DiscussionTypeEnum.ObjectTypeDialogAi
                ? await GetOwnerIdAsync(discussionType, discussionTypeId)
                : SCRUM_MASTER_AI_ID;

            // Выбираем Id диалога с владельцем.
            var ownerDialogId = await _chatRepository.GetDialogByUserIdAsync(ownerId,
                    discussionType == DiscussionTypeEnum.ObjectTypeDialogAi);

            // Выбираем Id диалога с текущем пользователем.
            var currentDialogId = await _chatRepository.GetDialogByUserIdAsync(userId,
                    discussionType == DiscussionTypeEnum.ObjectTypeDialogAi);

            // Найдем диалог, в котором есть оба участника, отталкиваемся от текущего пользователя.
            var findDialogId = await _chatRepository.GetDialogMembersAsync(userId, discussionTypeId,
                discussionType == DiscussionTypeEnum.ObjectTypeDialogAi);

            if (findDialogId == 0)
            {
                // TODO: Добавить удаление пустого диалога с шаблонным текстом в чат проекта, если пользователь
                // TODO: не писал в него сутки. Чтобы не плодить пустые диалоги.
                // Создаем новый диалог.
                dialogId = await _chatRepository.CreateDialogAsync(string.Empty, DateTime.UtcNow,
                    isManualNewDialog && discussionType == DiscussionTypeEnum.ObjectTypeDialogAi);
                    
                if (!dialogId.HasValue)
                {
                    throw new InvalidOperationException("Не удалось создать диалог либо у него битые данные.");
                }

                // Добавляем участников нового диалога.
                await _chatRepository.AddDialogMembersAsync(userId,
                    discussionType == DiscussionTypeEnum.ObjectTypeDialogAi ? ownerDialogId : ownerId, dialogId.Value,
                    discussionType == DiscussionTypeEnum.ObjectTypeDialogAi);
                
                result.DialogState = DialogStateEnum.Open.ToString();
                result.DialogId = dialogId.Value;

                return result;
            }
            
            var isFindDialog = false;

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
                // TODO: Добавить удаление пустого диалога с шаблонным текстом в чат проекта, если пользователь
                // TODO: не писал в него сутки. Чтобы не плодить пустые диалоги.
                // Создаем новый диалог.
                dialogId = await _chatRepository.CreateDialogAsync(string.Empty, DateTime.UtcNow,
                    isManualNewDialog && discussionType == DiscussionTypeEnum.ObjectTypeDialogAi);
                
                if (!dialogId.HasValue)
                {
                    throw new InvalidOperationException("Не удалось создать диалог либо у него битые данные.");
                }

                // Добавляем участников нового диалога.
                await _chatRepository.AddDialogMembersAsync(userId, ownerId, dialogId.Value,
                    discussionType == DiscussionTypeEnum.ObjectTypeDialogAi);
                
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
                result.FullName = discussionType == DiscussionTypeEnum.ObjectTypeDialogAi
                    ? "Scrum Master AI"
                    : await CreateDialogOwnerFioAsync(userId);

                return result;
            }

            dialogId ??= ownerDialogId; // Если dialogId == null, то берем ownerDialogId.

            // Проверяем существование диалога.
            var checkDialog = await _chatRepository.CheckDialogAsync(dialogId.Value,
                discussionType == DiscussionTypeEnum.ObjectTypeDialogAi);

            if (!checkDialog)
            {
                throw new InvalidOperationException($"Такого диалога не найдено. DialogId был {dialogId.Value}");
            }

            // Получаем список Id участников диалога.
            var memberIds = await _chatRepository.GetDialogMembersAsync(dialogId.Value,
                discussionType == DiscussionTypeEnum.ObjectTypeDialogAi);

            if (memberIds.Count == 0)
            {
                throw new InvalidOperationException($"Не найдено участников для диалога с DialogId {dialogId.Value}");
            }
            
            var user = await _userRepository.GetUserByUserIdAsync(userId);

            // Записываем полное ФИО пользователя, с которым идет общение в чате.
            if (user.FirstName is not null && user.LastName is not null)
            {
                result.FirstName = user.FirstName;
                result.LastName = user.LastName;   
            }

            // Исключаем текущего пользователя.
            var newMembers = memberIds.Distinct();

            // Если там было 2 дубля пользователя, то выберем просто текущего.
            // Иначе берем первого исключая текущего.
            var id = newMembers.Count() <= 1 ? userId : memberIds.Except(new[] { userId }).First();
            
            result.FullName = await CreateDialogOwnerFioAsync(id);
            result.DialogId = dialogId.Value;
            
            // Получаем дату начала диалога.
            result.DateStartDialog = await _chatRepository.GetDialogStartDateAsync(dialogId.Value,
                discussionType == DiscussionTypeEnum.ObjectTypeDialogAi);
            
            result.DialogState = DialogStateEnum.Open.ToString();
            
            // Получаем список сообщений диалога.
            var dialogMessages = await _chatRepository.GetDialogMessagesAsync(dialogId.Value,
                discussionType == DiscussionTypeEnum.ObjectTypeDialogAi);

            // Если у диалога нет сообщений, значит вернуть пустой диалог, который будет открыт.
            if (dialogMessages.Count == 0)
            {
                result.DialogState = DialogStateEnum.Empty.ToString();

                return result;
            }

            // Работаем с сообщениями диалога.
            foreach (var item in dialogMessages)
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

    /// <inheritdoc />
    public async Task<DialogResultOutput> WriteProjectDialogOwnerAsync(DiscussionTypeEnum discussionType,
        string account, long discussionTypeId, string token)
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

            // Найдем диалог, в котором есть оба участника.
            var findDialogId = await _chatRepository.GetDialogMembersAsync(userId, discussionTypeId, false);
            
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
            var getDialogId = await _chatRepository.CheckDialogAsync(userId, ownerId);

            // Диалога нет, можем создавать.
            if (getDialogId is null)
            {
                // Создаем новый диалог.
                var lastDialogId = await _chatRepository.CreateDialogAsync(string.Empty, DateTime.UtcNow, false);
                
                if (!lastDialogId.HasValue)
                {
                    throw new InvalidOperationException("Не удалось создать диалог либо у него битые данные.");
                }

                // Добавляем участников нового диалога.
                await _chatRepository.AddDialogMembersAsync(userId, ownerId, lastDialogId.Value, false);
                result.DialogState = DialogStateEnum.Open.ToString();
                result.DialogId = lastDialogId.Value;

                var dialogId = result.DialogId;

                // Получаем дату начала диалога.
                result.DateStartDialog = await _chatRepository.GetDialogStartDateAsync(dialogId, false);
                
                // Устанавливаем связь, если диалог создается для проекта.
                if (discussionType == DiscussionTypeEnum.Project)
                {
                    // Связываем диалог с проектом.
                    await _chatRepository.SetReferenceProjectDialogAsync(dialogId, discussionTypeId);
                    result.ProjectId = discussionTypeId;
                    
                    // Связываем диалог с вакансией (если при отклике на проект, отклик был с указанием вакансии).
                    await _projectResponseRepository.SetReferenceVacancyDialogAsync(discussionTypeId, userId);
                    
                    // Генерим автоматическое сообщение от системы, потому что для отображения диалога на фронте,
                    // нужно минимум 1 сообщение.
                    await SendMessageAsync(
                        "Начало обсуждения с владельцем проекта. Это сообщение создано автоматически.", dialogId,
                        userId, token, false, false)
                        .ConfigureAwait(false);
                }
                
                return result;
            }
            
            result.DialogState = DialogStateEnum.Empty.ToString();
            result.DialogId = (long)getDialogId;
            result.FullName = await CreateDialogOwnerFioAsync(ownerId);

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<DialogResultOutput> SendMessageAsync(string message, long dialogId, long userId, string token,
        bool isMyMessage, bool isScrumMasterAi)
    {
        try
        {
            // Проверяем существование диалога.
            var checkDialog = await _chatRepository.CheckDialogAsync(dialogId, isScrumMasterAi);

            if (!checkDialog)
            {
                throw new InvalidOperationException($"Такого диалога не найдено. DialogId был {dialogId}");
            }

            // Записываем сообщение в БД.
            await _chatRepository.SaveMessageAsync(message, dialogId, DateTime.UtcNow, userId, isMyMessage,
                isScrumMasterAi);

            // Получаем список сообщений диалога.
            var messages = await _chatRepository.GetDialogMessagesAsync(dialogId, isScrumMasterAi);

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
            
            // Дополнительная обработка сообщений.
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