using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Services.Abstractions.Header;
using LeokaEstetica.Platform.Services.Abstractions.Landing;
using LeokaEstetica.Platform.Services.Services.Header;
using LeokaEstetica.Platform.Services.Services.Landing;

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
            
            // Сервис лендингов.
            builder.RegisterType<LandingService>().Named<ILandingService>("LandingService");
            builder.RegisterType<LandingService>().As<ILandingService>();
        }
    }
}