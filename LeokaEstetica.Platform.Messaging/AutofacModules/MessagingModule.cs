using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Messaging.Abstractions.Mail;
using LeokaEstetica.Platform.Messaging.Services.Mail;

namespace LeokaEstetica.Platform.Messaging.AutofacModules;

[CommonModule]
public class MessagingModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        // Сервис Email-сообщений.
        builder.RegisterType<MailingsService>().Named<IMailingsService>("MailingsService").InstancePerLifetimeScope();
    }
}