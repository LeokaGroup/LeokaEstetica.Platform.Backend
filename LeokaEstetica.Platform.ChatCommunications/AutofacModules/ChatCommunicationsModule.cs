using Autofac;
using LeokaEstetica.Platform.Communications.Abstractions;
using LeokaEstetica.Platform.Communications.Services;
using LeokaEstetica.Platform.Core.Attributes;

namespace LeokaEstetica.Platform.Communications.AutofacModules;

[CommonModule]
public class ChatCommunicationsModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        // Репозиторий хидера.
        builder.RegisterType<AbstractScopeService>()
            .Named<IAbstractScopeService>("AbstractScopeService")
            .InstancePerLifetimeScope();
        builder.RegisterType<AbstractScopeService>()
            .As<IAbstractScopeService>()
            .InstancePerLifetimeScope();
    }
}