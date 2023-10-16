using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Processing.Abstractions.Commerce;
using LeokaEstetica.Platform.Processing.Abstractions.PayMaster;
using LeokaEstetica.Platform.Processing.Abstractions.YandexKassa;
using LeokaEstetica.Platform.Processing.Models;
using LeokaEstetica.Platform.Processing.Services.Commerce;
using LeokaEstetica.Platform.Processing.Services.PayMaster;
using LeokaEstetica.Platform.Processing.Services.YandexKassa;
using LeokaEstetica.Platform.Processing.Strategies.PaymentSystem;

namespace LeokaEstetica.Platform.Processing.AutofacModules;

[CommonModule]
public class ProcessingModule : Module
{
    public static void InitModules(ContainerBuilder builder)
    {
        // Сервис платежной системы PayMaster.
        builder.RegisterType<PayMasterService>()
            .Named<IPayMasterService>("PayMasterService")
            .InstancePerLifetimeScope();
        builder.RegisterType<PayMasterService>()
            .As<IPayMasterService>()
            .InstancePerLifetimeScope();

        // Сервис коммерции.
        builder.RegisterType<CommerceService>()
            .Named<ICommerceService>("CommerceService")
            .InstancePerLifetimeScope();
        builder.RegisterType<CommerceService>()
            .As<ICommerceService>()
            .InstancePerLifetimeScope();

        // Класс стратегии платежной системы ЮKassa.
        builder.RegisterType<YandexKassaStrategy>()
            .Named<BasePaymentSystemStrategy>("YandexKassaStrategy")
            .InstancePerLifetimeScope();

        // Класс стратегии платежной системы PayMaster.
        builder.RegisterType<PayMasterStrategy>()
            .Named<BasePaymentSystemStrategy>("PayMasterStrategy")
            .InstancePerLifetimeScope();

        // Сервис платежной системы ЮKassa.
        builder.RegisterType<YandexKassaService>()
            .Named<IYandexKassaService>("YandexKassaService")
            .InstancePerLifetimeScope();
        builder.RegisterType<YandexKassaService>()
            .As<IYandexKassaService>()
            .InstancePerLifetimeScope();
        
        builder.RegisterType<IPaymentOrderReference>()
            .InstancePerLifetimeScope();
    }
}