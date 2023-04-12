using LeokaEstetica.Platform.WorkerServices.Jobs.User;
using Quartz;
using Quartz.Impl;

namespace LeokaEstetica.Platform.WorkerServices.Schedulers.User;

/// <summary>
/// Класс планировщика работы джобы UserActivityJob.
/// </summary>
public class UserActivityScheduler
{
    public static async void Start()
    {
        var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
        await scheduler.Start();

        var job = JobBuilder.Create<UserActivityJob>().Build();

        var trigger = TriggerBuilder.Create() // Создаем триггер.
            .WithIdentity("UserActivityTrigger", "UserActivityGroup") // Идентифицируем триггер с именем и группой.
            .StartNow() // Запуск сразу после начала выполнения.
            .WithSimpleSchedule(x => x // Настраиваем выполнение действия.
                .WithIntervalInHours(24) // Раз в сутки.
                .RepeatForever()) // Бесконечное повторение.
            .Build(); // Создаем триггер.

        // Начинаем выполнение работы.
        await scheduler.ScheduleJob(job, trigger);
    }
}