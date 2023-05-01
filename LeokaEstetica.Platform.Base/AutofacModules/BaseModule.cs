using Autofac;
using LeokaEstetica.Platform.Base.Abstractions.Messaging.Mail;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Common;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Validation;
using LeokaEstetica.Platform.Base.Abstractions.Services.Messaging.Mail;
using LeokaEstetica.Platform.Base.Abstractions.Services.Validation;
using LeokaEstetica.Platform.Base.Repositories.Common;
using LeokaEstetica.Platform.Base.Repositories.Validation;
using LeokaEstetica.Platform.Base.Services.Validation;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Models.Entities.Moderation;

namespace LeokaEstetica.Platform.Base.AutofacModules;

[CommonModule]
public class BaseModule<T> : Module
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
        
        // TODO: Надо будет настроить такую регистрацию в AutoFac.
        // Репозиторий обновления сущностей generic-типов.
        // builder
        //     .RegisterGeneric(typeof(UpdateDetachedEntitiesRepository<>))
        //     .As(typeof(IUpdateDetachedEntitiesRepository<>))
        //     .InstancePerLifetimeScope();
    }
}