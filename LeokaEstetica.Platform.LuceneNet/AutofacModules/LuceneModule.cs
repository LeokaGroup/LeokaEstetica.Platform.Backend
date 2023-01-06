using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.LuceneNet.Abstractions.Vacancy;
using LeokaEstetica.Platform.LuceneNet.Services.Vacancy;

namespace LeokaEstetica.Platform.LuceneNet.AutofacModules;

[CommonModule]
public class LuceneModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        // Сервис поиска и фильтрации вакансий.
        builder.RegisterType<VacancyFinderService>()
            .Named<IVacancyFinderService>("VacancyFinderService")
            .SingleInstance();
    }
}