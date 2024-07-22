using LeokaEstetica.Platform.Core.Attributes;
using Autofac;

namespace LeokaEstetica.Platform.Database.MongoDb.AutofacModules;

// TODO: Регаем пока в Program, иначе передавать придется настройки БД.
[CommonModule]
public class MongoDbModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        // Репозиторий хидера.
        // builder.RegisterType<MongoDbRepository>()
        //     .Named<IMongoDbRepository>("MongoDbRepository")
        //     .InstancePerLifetimeScope();
        // builder.RegisterType<MongoDbRepository>()
        //     .As<IMongoDbRepository>()
        //     .InstancePerLifetimeScope();
    }
}