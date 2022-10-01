using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Services.Abstractions.Header;
using LeokaEstetica.Platform.Services.Services.Header;

namespace LeokaEstetica.Platform.Services.AutofacModules;

public class ServicesModule
{
    [CommonModule]
    public class LogsModule : Module
    {
        public static void InitModules(ContainerBuilder builder)
        {
            // Сервис хидера.
            builder.RegisterType<HeaderService>().Named<IHeaderService>("HeaderService");
            builder.RegisterType<HeaderService>().As<IHeaderService>();
        }
    }
}