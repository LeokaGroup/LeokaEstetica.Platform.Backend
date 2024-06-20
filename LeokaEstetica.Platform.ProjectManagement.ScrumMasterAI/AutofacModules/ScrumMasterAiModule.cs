using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.ProjectManagement.ScrumMasterAI.Abstractions;
using LeokaEstetica.Platform.ProjectManagement.ScrumMasterAI.Services;

namespace LeokaEstetica.Platform.ProjectManagement.ScrumMasterAI.AutofacModules;

[CommonModule]
public class ScrumMasterAiModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        // Сервис нейросети Scrum Master AI.
        builder.RegisterType<ScrumMasterAiService>()
            .Named<IScrumMasterAiService>("ScrumMasterAiService")
            .InstancePerLifetimeScope();
        builder.RegisterType<ScrumMasterAiService>()
            .As<IScrumMasterAiService>()
            .InstancePerLifetimeScope();
    }
}