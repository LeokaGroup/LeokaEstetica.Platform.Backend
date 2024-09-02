using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Finder.Abstractions.Project;
using LeokaEstetica.Platform.Finder.Abstractions.Resume;
using LeokaEstetica.Platform.Finder.Services.Project;
using LeokaEstetica.Platform.Finder.Services.Resume;

namespace LeokaEstetica.Platform.Finder.AutofacModules;

[CommonModule]
public class FinderModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        // Поисковый сервис резюме.
        builder
            .RegisterType<ResumeFinderService>()
            .Named<IResumeFinderService>("ResumeFinderService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<ResumeFinderService>()
            .As<IResumeFinderService>()
            .InstancePerLifetimeScope();
        
        builder
            .RegisterType<ProjectPaginationService>()
            .Named<IProjectPaginationService>("ProjectPaginationService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<ProjectPaginationService>()
            .As<IProjectPaginationService>()
            .InstancePerLifetimeScope();
        
        builder
            .RegisterType<ResumePaginationService>()
            .Named<IResumePaginationService>("ResumePaginationService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<ResumePaginationService>()
            .As<IResumePaginationService>()
            .InstancePerLifetimeScope();
    }
}