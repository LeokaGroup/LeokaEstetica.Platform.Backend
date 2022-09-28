using System.Reflection;
using Autofac;
using AutoMapper;
using LeokaEstetica.Platform.Core.Attributes;
using LeokaEstetica.Platform.Core.Data;
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
        Func<string, bool> isAssembly = file =>
            string.Equals(Path.GetExtension(file), ".dll", StringComparison.OrdinalIgnoreCase);

        return Directory.GetFiles(baseDirectoryPath)
            .Where(isAssembly)
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
        var assemblies1 =
            GetAssembliesFromApplicationBaseDirectory(x =>
                x.FullName.StartsWith("LeokaEstetica.Platform.Logs"));
        
        var assemblies2 =
            GetAssembliesFromApplicationBaseDirectory(x =>
                x.FullName.StartsWith("LeokaEstetica.Platform.Services"));
        
        var assemblies3 =
            GetAssembliesFromApplicationBaseDirectory(x =>
                x.FullName.StartsWith("LeokaEstetica.Platform.Base"));
        
        var assemblies4 =
            GetAssembliesFromApplicationBaseDirectory(x =>
                x.FullName.StartsWith("LeokaEstetica.Platform.Database"));

        b.RegisterAssemblyTypes(assemblies1).AsImplementedInterfaces();
        b.RegisterAssemblyTypes(assemblies2).AsImplementedInterfaces();
        b.RegisterAssemblyTypes(assemblies3).AsImplementedInterfaces();
        b.RegisterAssemblyTypes(assemblies4).AsImplementedInterfaces();
        
        var assemblies = assemblies1
            .Union(assemblies2)
            .Union(assemblies3)
            .Union(assemblies4);

        RegisterMapper(b);

        _typeModules = (from assembly in assemblies
            from type in assembly.GetTypes()
            where type.IsClass && type.GetCustomAttribute<CommonModuleAttribute>() != null
            select type).ToArray();
        
        foreach (var module in _typeModules)
        {
            if (module is not null)
            {
                b.RegisterModule(Activator.CreateInstance(module) as Module);   
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

    private static void RegisterMapper(ContainerBuilder builder)
    {
        builder.RegisterType<MappingProfile>().As<Profile>();
        builder.Register(c => new MapperConfiguration(cfg =>
        {
            foreach (var profile in c.Resolve<IEnumerable<Profile>>())
            {
                cfg.AddProfile(profile);
            }
        })).AsSelf().SingleInstance();
    
        builder.Register(c => c.Resolve<MapperConfiguration>().CreateMapper(c.Resolve))
            .As<IMapper>()
            .InstancePerLifetimeScope();
    }
}