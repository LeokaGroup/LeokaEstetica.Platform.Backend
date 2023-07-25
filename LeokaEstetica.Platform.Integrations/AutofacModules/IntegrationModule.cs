using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Integrations.Abstractions.Telegram;
using LeokaEstetica.Platform.Integrations.Services.Telegram;

namespace LeokaEstetica.Platform.Integrations.AutofacModules;

[CommonModule]
public class IntegrationModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        // Сервис телеграм бота.
        builder.RegisterType<TelegramBotService>()
            .Named<ITelegramBotService>("TelegramBotService")
            .InstancePerLifetimeScope();
        builder.RegisterType<TelegramBotService>()
            .As<ITelegramBotService>()
            .InstancePerLifetimeScope();
    }
}