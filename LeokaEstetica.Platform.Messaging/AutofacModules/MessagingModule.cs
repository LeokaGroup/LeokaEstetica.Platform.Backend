using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Messaging.Abstractions.Chat;
using LeokaEstetica.Platform.Messaging.Abstractions.Mail;
using LeokaEstetica.Platform.Messaging.Abstractions.Project;
using LeokaEstetica.Platform.Messaging.Abstractions.RabbitMq;
using LeokaEstetica.Platform.Messaging.Abstractions.Ticket;
using LeokaEstetica.Platform.Messaging.Services.Chat;
using LeokaEstetica.Platform.Messaging.Services.Mail;
using LeokaEstetica.Platform.Messaging.Services.Project;
using LeokaEstetica.Platform.Messaging.Services.RabbitMq;
using LeokaEstetica.Platform.Messaging.Services.Ticket;

namespace LeokaEstetica.Platform.Messaging.AutofacModules;

[CommonModule]
public class MessagingModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        // Сервис Email-сообщений.
        builder.RegisterType<MailingsService>()
            .Named<IMailingsService>("MailingsService")
            .InstancePerLifetimeScope();
        
        // Сервис чата.
        builder.RegisterType<ChatService>()
            .Named<IChatService>("ChatService")
            .InstancePerLifetimeScope();
        
        // Сервис комментариев к проектам.
        builder.RegisterType<ProjectCommentsService>()
            .Named<IProjectCommentsService>("ProjectCommentsService")
            .InstancePerLifetimeScope();
        
        // Сервис RabbitMQ.
        builder.RegisterType<RabbitMqService>()
            .Named<IRabbitMqService>("RabbitMqService")
            .SingleInstance();
        
        // Сервис сообщений тикетов.
        builder.RegisterType<TicketMessagesService>()
            .Named<ITicketMessagesService>("TicketMessagesService")
            .InstancePerLifetimeScope();
        builder.RegisterType<TicketMessagesService>()
            .As<ITicketMessagesService>()
            .InstancePerLifetimeScope();
    }
}