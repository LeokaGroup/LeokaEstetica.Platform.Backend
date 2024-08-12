using System.Reflection;
using Autofac;
using Autofac.Core;
using AutoMapper;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Core.Mapper;
using Microsoft.EntityFrameworkCore;
using Module = Autofac.Module;

namespace LeokaEstetica.Platform.Core.Utils;

/// <summary>
/// Класс конфигурации и зависимостей автофака.
/// </summary>
public static class AutoFac
{
    private static ContainerBuilder _builder;
    private static IContainer _container;
    private static IEnumerable<Type> _typeModules;
    
    // Сборки, в которых надо регистрировать зависимости.
    // Для добавления новой регистрации, достаточно просто добавить сюда название сборки.
    private static readonly List<string> _conditions = new()
    {
        "LeokaEstetica.Platform.Logs",
        "LeokaEstetica.Platform.Services",
        "LeokaEstetica.Platform.Base",
        "LeokaEstetica.Platform.Database",
        "LeokaEstetica.Platform.Access",
        "LeokaEstetica.Platform.Messaging",
        "LeokaEstetica.Platform.Notifications",
        "LeokaEstetica.Platform.Redis",
        "LeokaEstetica.Platform.CallCenter",
        "LeokaEstetica.Platform.Finder",
        "LeokaEstetica.Platform.Processing",
        "LeokaEstetica.Platform.Diagnostics",
        "LeokaEstetica.Platform.Integrations",
        "LeokaEstetica.Platform.ProjectManagement.Documents",
        "LeokaEstetica.Platform.ProjectManagment.Documents",
        "LeokaEstetica.Platform.RabbitMq",
        "LeokaEstetica.Platform.ProjectManagement.ScrumMasterAI",
        "LeokaEstetica.Platform.Database.MongoDb"
    };

    private static readonly List<Assembly> _assemblies = new();

    /// <summary>
    /// Метод инициализирует контейнер начальными регистрациями.
    /// </summary>
    public static void Init(ContainerBuilder b)
    {
        RegisterAllAssemblyTypes(b);
    }

    /// <summary>
    /// Метод получит все сборки для сканирования.
    /// </summary>
    /// <param name="condition">Условие сканирования, например конкретная сборка.</param>
    /// <returns>Массив сборок.</returns>
    public static Assembly[] GetAssembliesFromApplicationBaseDirectory(Func<AssemblyName, bool> condition)
    {
        var baseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;

        bool IsAssembly(string file) =>
            string.Equals(Path.GetExtension(file), ".dll", StringComparison.OrdinalIgnoreCase);

        return Directory.GetFiles(baseDirectoryPath)
            .Where((Func<string, bool>)IsAssembly)
            .Where(f => condition(AssemblyName.GetAssemblyName(f)))
            .Select(Assembly.LoadFrom)
            .ToArray();
    }

    /// <summary>
    /// Метод сканирует все сборки и регистрирует все модули, которые найдет в них.
    /// </summary>
    /// <param name="builder">Билдер контейнера, который наполнять регистрациями.</param>
    private static void RegisterAllAssemblyTypes(ContainerBuilder b)
    {
        foreach (var c in _conditions)
        {
            var assembly = GetAssembliesFromApplicationBaseDirectory(x => x.FullName.StartsWith(c));
            
            b.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();

            _assemblies.AddRange(assembly);
        }

        RegisterMapper(b);
        
        // Регистрация моделей, которые будут резолвиться.
        ModelsExtensions.RegisterModels(b);

        _typeModules = (from assembly in _assemblies
            from type in assembly.GetTypes()
            where type.IsClass && type.GetCustomAttribute<CommonModuleAttribute>() is not null
            select type).ToArray();

        foreach (var module in _typeModules)
        {
            if (module is not null)
            {
                b.RegisterModule((Activator.CreateInstance(module) as Module)!);
            }
        }
    }

    /// <summary>
    /// Получить сервис
    /// </summary>
    /// <typeparam name="TService">Тип сервиса</typeparam>
    /// <param name="notException">Не выдавать исключение если не удалось получить объект По умолчанию false</param> 
    /// <returns>Экземпляр запрашиваемого сервиса</returns>
    public static TService Resolve<TService>() where TService : class
    {
        if (_container == null)
        {
            _builder = new ContainerBuilder();

            RegisterAllAssemblyTypes(_builder);
            RegisterDbContext(_builder);
            RegisterMapper(_builder);
            
            // Регистрация моделей, которые будут резолвиться.
            ModelsExtensions.RegisterModels(_builder);

            _container = _builder.Build();
        }

        var service = _container.Resolve<TService>();

        return service;
    }

    public static ILifetimeScope CreateLifetimeScope()
    {
        if (_container == null)
        {
            _builder = new ContainerBuilder();
            _container = _builder.Build();
        }

        RegisterAllAssemblyTypes(_builder);
        RegisterDbContext(_builder);
        RegisterMapper(_builder);
        
        // Регистрация моделей, которые будут резолвиться.
        ModelsExtensions.RegisterModels(_builder);

        return _container.BeginLifetimeScope();
    }

    /// <summary>
    /// Получить сервис по уникальному имени
    /// </summary>
    /// <typeparam name="TService">Экземпляр запрашиваемого сервиса</typeparam>
    /// <param name="serviceName">Уникальное имя запрашиваемого типа</param>
    /// <param name="notException">Не выдавать исключение если не удалось получить объект По умолчанию false</param>
    /// <returns></returns>
    public static TService ResolveNamedScoped<TService>(this ILifetimeScope scope, string serviceName)
        where TService : class
    {
        if (!_container.IsRegisteredWithName<TService>(serviceName))
        {
            return null;
        }

        var service = _container.ResolveNamed<TService>(serviceName);

        return service;
    }
    
    public static T[] ResolveAllWithParameters<T>(this IContainer Container, IDictionary<string, object> parameters)
    {
        var _parameters = new List<Parameter>();
        
        foreach (var parameter in parameters)
        {
            _parameters.Add(new NamedParameter(parameter.Key, parameter.Value));
        }
        
        return Container.Resolve<IEnumerable<T>>(_parameters).ToArray();
    }

    /// <summary>
    /// Метод регистрирует контекст БД.
    /// </summary>
    /// <param name="builder">Билдер контейнера, который наполнять регистрациями.</param>
    private static void RegisterDbContext(ContainerBuilder builder)
    {
        var optionsBuilder = new DbContextOptions<PgContext>();
        builder.RegisterType<PgContext>()
            .WithParameter("options", optionsBuilder)
            .InstancePerLifetimeScope();
    }

    /// <summary>
    /// Метод регистрирует маппер.
    /// </summary>
    /// <param name="builder">Билдер контейнера, который наполнять регистрациями.</param>
    public static void RegisterMapper(ContainerBuilder builder)
    {
        builder.RegisterType<MappingProfile>().As<Profile>();
        builder.Register(c => new MapperConfiguration(cfg =>
            {
                foreach (var profile in c.Resolve<IEnumerable<Profile>>())
                {
                    cfg.AddProfile(profile);
                }
            }))
            .AsSelf()
            .SingleInstance();

        builder.Register(c => c.Resolve<MapperConfiguration>().CreateMapper(c.Resolve))
            .As<IMapper>()
            .InstancePerLifetimeScope();
    }
}