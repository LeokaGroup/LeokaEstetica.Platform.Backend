using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Finder.Abstractions.Project;
using LeokaEstetica.Platform.Finder.Abstractions.Resume;
using LeokaEstetica.Platform.Finder.Abstractions.Vacancy;
using LeokaEstetica.Platform.Finder.Services.Project;
using LeokaEstetica.Platform.Finder.Services.Resume;
using LeokaEstetica.Platform.Finder.Services.Vacancy;

namespace LeokaEstetica.Platform.Finder.AutofacModules;

[CommonModule]
public class FinderModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        // Поисковый сервис вакансий.
        builder
            .RegisterType<VacancyFinderService>()
            .Named<IVacancyFinderService>("VacancyFinderService")
            .SingleInstance();
        builder
            .RegisterType<VacancyFinderService>()
            .As<IVacancyFinderService>()
            .SingleInstance();
        
        // Поисковый сервис проектов.
        builder
            .RegisterType<ProjectFinderService>()
            .Named<IProjectFinderService>("ProjectFinderService")
            .SingleInstance();
        builder
            .RegisterType<ProjectFinderService>()
            .As<IProjectFinderService>()
            .SingleInstance();
        
        // Поисковый сервис резюме.
        builder
            .RegisterType<ResumeFinderService>()
            .Named<IResumeFinderService>("ResumeFinderService")
            .SingleInstance();
        builder
            .RegisterType<ResumeFinderService>()
            .As<IResumeFinderService>()
            .SingleInstance();
        
        // Сервис пагинации вакансий.
        builder
            .RegisterType<VacancyPaginationService>()
            .Named<IVacancyPaginationService>("VacancyPaginationService")
            .SingleInstance();
        builder
            .RegisterType<VacancyPaginationService>()
            .As<IVacancyPaginationService>()
            .SingleInstance();
    }
}