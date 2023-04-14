using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Messaging.Abstractions.Mail;
using LeokaEstetica.Platform.Models.Entities.User;
using LeokaEstetica.Platform.Redis.Abstractions.User;

namespace LeokaEstetica.Platform.WorkerServices.Jobs.User;

/// <summary>
/// Класс джобы проставления пометки к удалению аккаунтов пользователей, если они не заходили более 30 дней.
/// </summary>
public class UserActivityMarkDeactivateJob : BackgroundService
{
    private Timer _timer;
    private readonly IUserRepository _userRepository;
    private readonly IUserRedisService _userRedisService;
    private readonly ILogService _logService;
    private readonly IMailingsService _mailingsService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="userRedisService">Сервис кэша.</param>
    /// <param name="logService">Сервис логов.</param>
    /// <param name="mailingsService">Сервис уведомлений на почту.</param>
    public UserActivityMarkDeactivateJob(IUserRepository userRepository,
        IUserRedisService userRedisService,
        ILogService logService,
        IMailingsService mailingsService)
    {
        _userRepository = userRepository;
        _userRedisService = userRedisService;
        _logService = logService;
        _mailingsService = mailingsService;
    }

    /// <summary>
    /// Метод запускает логику фоновой задачи.
    /// </summary>
    /// <param name="stoppingToken">Токен отмены.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

        await Task.CompletedTask;
    }

    /// <summary>
    /// Метод выполняет логику фоновой задачи.
    /// </summary>
    /// <param name="state">Состояние.</param>
    private async void DoWork(object state)
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            var now = DateTime.Now;
            var markedUsers = new List<UserEntity>();
            var deletedUsers = new List<UserEntity>();

            foreach (var u in users)
            {
                // Вычисляемм разницу дат.
                var diffDates = now.Subtract(u.LastAutorization).TotalDays;

                // Если дата последней авторизации > 30 дней,
                // то вышлем на почту предупреждение о удалении аккаунта пользователя через 1 неделю.
                // Также помечаем пользователя в БД меткой.
                if (diffDates <= 30)
                {
                    continue;
                }

                // Прошел месяц, даем шанс вернуться и авторизоваться на платформе, если не более срока, который даем на шанс.
                // То есть +1 неделя к 30 дням.
                if (diffDates is > 30 and <= 37)
                {
                    // Помечаем пользователей, которым проставим метку в БД и отправим предупреждение на почту.
                    u.IsMarkDeactivate = true;
                    u.DateCreatedMark = DateTime.Now;
                    markedUsers.Add(u);
                }

                // Шанс уже давали. Теперь запишем в кэш пользователей, которых будем удалять.
                else
                {
                    deletedUsers.Add(u);
                }
            }

            // Нет аккаунтов для пометок.
            if (!markedUsers.Any())
            {
                await Task.CompletedTask;
            }

            // Проставляем метки в БД.
            await _userRepository.SetMarkDeactivateAccountsAsync(markedUsers);

            // Отправляем пользователям предупреждение на почту.
            var mailsTo = markedUsers.Select(u => u.Email).ToList();
            await _mailingsService.SendNotificationDeactivateAccountAsync(mailsTo);

            // Нет аккаунтов для удаления.
            if (!deletedUsers.Any())
            {
                await Task.CompletedTask;
            }

            // Записываем в кэш для удаления аккаунтов.
            await _userRedisService.AddMarkDeactivateUserAccountsAsync(deletedUsers);
        }

        catch (Exception ex)
        {
            await _logService.LogCriticalAsync(ex, "Ошибка при выполнении фоновой задачи UserActivityMarkDeactivate.");
            throw;
        }
    }

    public override Task StopAsync(CancellationToken stoppingToken)
    {
        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _timer?.Dispose();
    }
}