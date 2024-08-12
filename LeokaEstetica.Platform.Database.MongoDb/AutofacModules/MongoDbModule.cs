using LeokaEstetica.Platform.Core.Attributes;
using Autofac;
using LeokaEstetica.Platform.Database.MongoDb.Abstractions;
using LeokaEstetica.Platform.Database.MongoDb.Repositories;

namespace LeokaEstetica.Platform.Database.MongoDb.AutofacModules;

[CommonModule]
public class MongoDbModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        // Репозиторий хидера.
        builder.RegisterType<MongoDbRepository>()
            .Named<IMongoDbRepository>("MongoDbRepository")
            .InstancePerLifetimeScope();
        builder.RegisterType<MongoDbRepository>()
            .As<IMongoDbRepository>()
            .InstancePerLifetimeScope();
    }
}