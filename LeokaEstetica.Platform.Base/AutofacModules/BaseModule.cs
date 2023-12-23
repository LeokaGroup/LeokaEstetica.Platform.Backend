using Autofac;
using LeokaEstetica.Platform.Base.Abstractions.Messaging.Mail;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Chat;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Validation;
using LeokaEstetica.Platform.Base.Abstractions.Services.Messaging.Mail;
using LeokaEstetica.Platform.Base.Abstractions.Services.Pachca;
using LeokaEstetica.Platform.Base.Abstractions.Services.Validation;
using LeokaEstetica.Platform.Base.Factors;
using LeokaEstetica.Platform.Base.Repositories.Chat;
using LeokaEstetica.Platform.Base.Repositories.User;
using LeokaEstetica.Platform.Base.Repositories.Validation;
using LeokaEstetica.Platform.Base.Services.Pachca;
using LeokaEstetica.Platform.Base.Services.Validation;
using LeokaEstetica.Platform.Core.Attributes;

namespace LeokaEstetica.Platform.Base.AutofacModules;

[CommonModule]
public class BaseModule : Module
{
     public static void InitModules(ContainerBuilder builder)
    {
        // Сервис исключения параметров валидации.
        builder
            .RegisterType<ValidationExcludeErrorsService>()
            .Named<IValidationExcludeErrorsService>("ValidationExcludeErrorsService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<ValidationExcludeErrorsService>()
            .As<IValidationExcludeErrorsService>()
            .InstancePerLifetimeScope();
        
        // Репозиторий исключения параметров валидации.
        builder
            .RegisterType<ValidationExcludeErrorsRepository>()
            .Named<IValidationExcludeErrorsRepository>("ValidationExcludeErrorsRepository")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<ValidationExcludeErrorsRepository>()
            .As<IValidationExcludeErrorsRepository>()
            .InstancePerLifetimeScope();
        
        // Сервис уведомлений.
        builder
            .RegisterType<MailingsService>()
            .Named<IMailingsService>("MailingsService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<MailingsService>()
            .As<IMailingsService>()
            .InstancePerLifetimeScope();

        // Репозиторий пользователей.
        builder.RegisterType<UserRepository>()
            .Named<IUserRepository>("LandingRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<UserRepository>()
            .As<IUserRepository>()
            .InstancePerLifetimeScope();
        
        // Репозиторий чата.
        builder.RegisterType<ChatRepository>()
            .Named<IChatRepository>("ChatRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<ChatRepository>()
            .As<IChatRepository>()
            .InstancePerLifetimeScope();
        
        // Сервис пачки.
        builder.RegisterType<PachcaService>()
            .Named<IPachcaService>("PachcaService")
            .InstancePerLifetimeScope();
        builder.RegisterType<PachcaService>()
            .As<IPachcaService>()
            .InstancePerLifetimeScope();
        
        // Факторка транзакций.
        builder.RegisterType<TransactionScopeFactory>()
            .Named<ITransactionScopeFactory>("TransactionScopeFactory")
            .InstancePerLifetimeScope();
        builder.RegisterType<TransactionScopeFactory>()
            .As<ITransactionScopeFactory>()
            .InstancePerLifetimeScope();
    }
}