using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Services.Abstractions.Header;
using LeokaEstetica.Platform.Services.Abstractions.Landing;
using LeokaEstetica.Platform.Services.Abstractions.User;
using LeokaEstetica.Platform.Services.Services.Header;
using LeokaEstetica.Platform.Services.Services.Landing;
using LeokaEstetica.Platform.Services.Services.User;

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
            
            // Сервис пользователя.
            builder.RegisterType<UserService>().Named<IUserService>("UserService");
            builder.RegisterType<UserService>().As<IUserService>();
            
            // Сервис профиля пользователя.
            builder.RegisterType<ProfileService>().Named<IProfileService>("ProfileService");
            builder.RegisterType<ProfileService>().As<IProfileService>();
        }
    }
}