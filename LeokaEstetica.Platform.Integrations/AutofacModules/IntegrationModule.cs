using Autofac;
using LazyProxy.Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Integrations.Abstractions.Reverso;
using LeokaEstetica.Platform.Integrations.Abstractions.Telegram;
using LeokaEstetica.Platform.Integrations.Services.Discord;
using LeokaEstetica.Platform.Integrations.Services.Reverso;
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
        
        // Сервис телеграм.
        builder.RegisterType<TelegramService>()
            .Named<ITelegramService>("TelegramService")
            .InstancePerLifetimeScope();
        builder.RegisterType<TelegramService>()
            .As<ITelegramService>()
            .InstancePerLifetimeScope();
        
        // Сервис пачки.
        // builder.RegisterType<PachcaService>()
        //     .Named<IPachcaService>("PachcaService")
        //     .InstancePerLifetimeScope();
        // builder.RegisterType<PachcaService>()
        //     .As<IPachcaService>()
        //     .InstancePerLifetimeScope();
        
        // Сервис дискорда.
        builder.RegisterType<DiscordService>()
            .Named<IDiscordService>("DiscordService")
            .InstancePerLifetimeScope();
        builder.RegisterType<DiscordService>()
            .As<IDiscordService>()
            .InstancePerLifetimeScope();
        
        // Сервис транслитера ReversoAPI.
        // builder.RegisterType<ReversoService>()
        //     .Named<IReversoService>("ReversoService")
        //     .InstancePerLifetimeScope();
        // builder.RegisterType<ReversoService>()
        //     .As<IReversoService>()
        //     .InstancePerLifetimeScope();
        // builder.RegisterType<ReversoService>()
        //     .As<IReversoService>()
        //     .InstancePerLifetimeScope();

        builder.RegisterLazy<IReversoService, ReversoService>();
        // builder.RegisterLazy<IPachcaService, PachcaService>();
        builder.RegisterLazy<IDiscordService, DiscordService>();
    }
}