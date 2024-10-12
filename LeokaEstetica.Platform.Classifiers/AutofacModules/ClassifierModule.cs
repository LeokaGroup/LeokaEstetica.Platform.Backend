using Autofac;
using LeokaEstetica.Platform.Classifiers.Abstractions.Communications;
using LeokaEstetica.Platform.Classifiers.Services.Communications;
using LeokaEstetica.Platform.Core.Attributes;

namespace LeokaEstetica.Platform.Classifiers.AutofacModules;

[CommonModule]
public class ClassifierModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        builder.RegisterType<ClassifierAbstractGroupService>()
            .Named<IClassifierAbstractGroupService>("ClassifierAbstractGroupService")
            .InstancePerLifetimeScope();
        builder.RegisterType<ClassifierAbstractGroupService>()
            .As<IClassifierAbstractGroupService>()
            .InstancePerLifetimeScope();
    }
}