using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.ProjectManagement.HumanResources.Abstractions;
using LeokaEstetica.Platform.ProjectManagement.HumanResources.Services;

namespace LeokaEstetica.Platform.ProjectManagement.HumanResources.AutofacModules;

[CommonModule]
public class HumanResourcesModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        builder.RegisterType<CalendarService>()
            .Named<ICalendarService>("CalendarService")
            .InstancePerLifetimeScope();
        builder.RegisterType<CalendarService>()
            .As<ICalendarService>()
            .InstancePerLifetimeScope();
    }
}