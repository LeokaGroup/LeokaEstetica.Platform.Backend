using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Diagnostics.Abstractions.Metrics;
using LeokaEstetica.Platform.Diagnostics.Services.Metrics;

namespace LeokaEstetica.Platform.Diagnostics.AutofacModules;

[CommonModule]
public class MetricsModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        // Сервис метрик пользователей.
        builder
            .RegisterType<UserMetricsService>()
            .Named<IUserMetricsService>("UserMetricsService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<UserMetricsService>()
            .As<IUserMetricsService>()
            .InstancePerLifetimeScope();
        
        // Сервис метрик проектов.
        builder
            .RegisterType<ProjectMetricsService>()
            .Named<IProjectMetricsService>("ProjectMetricsService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<ProjectMetricsService>()
            .As<IProjectMetricsService>()
            .InstancePerLifetimeScope();
    }
}