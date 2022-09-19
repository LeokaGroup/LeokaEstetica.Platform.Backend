using Autofac;
using LeokaEstetica.Platform.Core.Attributes;

namespace LeokaEstetica.Platform.Services.AutofacModules;

public class ServicesModule
{
    [CommonModule]
    public class LogsModule : Module
    {
        public static void InitModules(ContainerBuilder builder)
        {
            // Сервис логирования.
            // builder.RegisterType<LogService>().Named<BaseLogService>("LogService");
            // builder.RegisterType<LogService>().As<BaseLogService>();
        }
    }
}