using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Database.Abstractions.Header;
using LeokaEstetica.Platform.Database.Repositories.Header;

namespace LeokaEstetica.Platform.Database.AutofacModules;

public class RepositoriesModule
{
    [CommonModule]
    public class LogsModule : Module
    {
        public static void InitModules(ContainerBuilder builder)
        {
            // Репозиторий хидера.
            builder.RegisterType<HeaderRepository>().Named<IHeaderRepository>("HeaderRepository");
            builder.RegisterType<HeaderRepository>().As<IHeaderRepository>();
        }
    }
}