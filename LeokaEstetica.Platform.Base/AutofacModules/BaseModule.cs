using Autofac;
using LeokaEstetica.Platform.Base.Abstractions.Repositories;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Validation;
using LeokaEstetica.Platform.Base.Abstractions.Services;
using LeokaEstetica.Platform.Base.Abstractions.Services.Validation;
using LeokaEstetica.Platform.Base.Repositories;
using LeokaEstetica.Platform.Base.Repositories.Validation;
using LeokaEstetica.Platform.Base.Services;
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
    }
}