using Autofac;
using LeokaEstetica.Platform.Models.Dto.Chat.Output;

namespace LeokaEstetica.Platform.Core.Extensions;

/// <summary>
/// Класс регистрирует все модели, которые будут резолвиться.
/// </summary>
public static class ModelsExtensions
{
   /// <summary>
   /// Метод регистрирует все модели.
   /// </summary>
   /// <param name="builder">Билдер.</param>
   public static void RegisterModels(ContainerBuilder builder)
   {
      builder.RegisterType<DialogOutput>().As<BaseDialogOutput>();
      builder.RegisterType<ProfileDialogOutput>().As<BaseDialogOutput>();
   }
}