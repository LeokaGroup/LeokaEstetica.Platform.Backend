using LeokaEstetica.Platform.Core.Attributes;
using Autofac;
using LazyProxy.Autofac;
using LeokaEstetica.Platform.Base.Factors;
using LeokaEstetica.Platform.ProjectManagment.Documents.Abstractions;
using LeokaEstetica.Platform.ProjectManagment.Documents.Services;

namespace LeokaEstetica.Platform.ProjectManagement.Documents.AutofacModules;

[CommonModule]
public class DocumentModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        builder.RegisterLazy<IFileManagerService, FileManagerService>();
        builder.RegisterLazy<ITransactionScopeFactory, TransactionScopeFactory>();

        builder.RegisterType<FileManagerService>()
            .Named<IFileManagerService>("FileManagerService")
            .InstancePerLifetimeScope();
        builder.RegisterType<FileManagerService>()
            .As<IFileManagerService>()
            .InstancePerLifetimeScope();
    }
}