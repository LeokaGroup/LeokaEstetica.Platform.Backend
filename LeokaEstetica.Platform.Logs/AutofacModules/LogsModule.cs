using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Logs.Services;

namespace LeokaEstetica.Platform.Logs.AutofacModules;

[CommonModule]
public class LogsModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        // Репозиторий логирования.
        builder.RegisterType<LogService>().Named<ILogService>("LogService");
        builder.RegisterType<LogService>().As<ILogService>();
    }
}