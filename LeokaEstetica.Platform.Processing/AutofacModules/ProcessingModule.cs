using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Processing.Abstractions.Commerce;
using LeokaEstetica.Platform.Processing.Abstractions.PayMaster;
using LeokaEstetica.Platform.Processing.Services.Commerce;
using LeokaEstetica.Platform.Processing.Services.PayMaster;

namespace LeokaEstetica.Platform.Processing.AutofacModules;

[CommonModule]
public class ProcessingModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        // Сервис платежной системы PayMaster.
        builder
            .RegisterType<PayMasterService>()
            .Named<IPayMasterService>("PayMasterService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<PayMasterService>()
            .As<IPayMasterService>()
            .InstancePerLifetimeScope();
        
        // Сервис коммерции.
        builder
            .RegisterType<CommerceService>()
            .Named<ICommerceService>("CommerceService")
            .InstancePerLifetimeScope();
        builder
            .RegisterType<CommerceService>()
            .As<ICommerceService>()
            .InstancePerLifetimeScope();
    }
}