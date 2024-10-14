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
        builder.RegisterType<AbstractScopeService>()
            .Named<IAbstractScopeService>("AbstractScopeService")
            .InstancePerLifetimeScope();
        builder.RegisterType<AbstractScopeService>()
            .As<IAbstractScopeService>()
            .InstancePerLifetimeScope();
            
        builder.RegisterType<AbstractGroupService>()
            .Named<IAbstractGroupService>("AbstractGroupService")
            .InstancePerLifetimeScope();
        builder.RegisterType<AbstractGroupService>()
            .As<IAbstractGroupService>()
            .InstancePerLifetimeScope();
        
        builder.RegisterType<AbstractGroupObjectsService>()
            .Named<IAbstractGroupObjectsService>("AbstractGroupObjectsService")
            .InstancePerLifetimeScope();
        builder.RegisterType<AbstractGroupObjectsService>()
            .As<IAbstractGroupObjectsService>()
            .InstancePerLifetimeScope();
    }
}