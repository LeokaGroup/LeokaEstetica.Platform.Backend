using Autofac;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Processing.Abstractions.Commerce;
using LeokaEstetica.Platform.Processing.Abstractions.PayMaster;
using LeokaEstetica.Platform.Processing.Abstractions.YandexKassa;
using LeokaEstetica.Platform.Processing.Builders.Order;
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

        // TODO: Не используется (в будущем возможно будет).
        // Класс стратегии платежной системы PayMaster.
        // builder.RegisterType<PayMasterStrategy>()
        //     .Named<BasePaymentSystemStrategy>("PayMasterStrategy")
        //     .InstancePerLifetimeScope();

        // Сервис платежной системы ЮKassa.
        builder.RegisterType<YandexKassaService>()
            .Named<IYandexKassaService>("YandexKassaService")
            .InstancePerLifetimeScope();
        builder.RegisterType<YandexKassaService>()
            .As<IYandexKassaService>()
            .InstancePerLifetimeScope();
            
        builder.RegisterType<FareRuleOrderBuilder>()
            .Named<BaseOrderBuilder>("FareRuleOrderBuilder")
            .InstancePerLifetimeScope();
        builder.RegisterType<FareRuleOrderBuilder>()
            .As<BaseOrderBuilder>()
            .InstancePerLifetimeScope();
            
        builder.RegisterType<PostVacancyOrderBuilder>()
            .Named<BaseOrderBuilder>("PostVacancyOrderBuilder")
            .InstancePerLifetimeScope();
        builder.RegisterType<PostVacancyOrderBuilder>()
            .As<BaseOrderBuilder>()
            .InstancePerLifetimeScope();
    }
}