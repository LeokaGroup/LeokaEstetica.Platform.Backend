using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.RabbitMq.Abstractions;
using LeokaEstetica.Platform.RabbitMq.Services;

namespace LeokaEstetica.Platform.RabbitMq.AutofacModules;

[CommonModule]
public class RabbitMqModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        builder.RegisterType<RabbitMqService>()
            .Named<IRabbitMqService>("RabbitMqService")
            .SingleInstance();
        builder.RegisterType<RabbitMqService>()
            .As<IRabbitMqService>()
            .InstancePerLifetimeScope();
    }
}