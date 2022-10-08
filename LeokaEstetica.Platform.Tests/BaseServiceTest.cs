using Autofac;
using AutoMapper;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Utils;
using LeokaEstetica.Platform.Database.Repositories.User;
using LeokaEstetica.Platform.Logs.Services;
using LeokaEstetica.Platform.Services.Services.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LeokaEstetica.Platform.Tests;

/// <summary>
/// Базовый класс тестов с настройками конфигурации.
/// </summary>
public class BaseServiceTest
{
    protected string PostgreConfigString { get; set; }
    protected IConfiguration AppConfiguration { get; set; }
    protected PgContext PgContext;
    protected LogService LogService;
    protected UserService UserService;
    protected UserRepository UserRepository;
    
    public BaseServiceTest()
    {
        // Настройка тестовых строк подключения.
        var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
        AppConfiguration = builder.Build();
        PostgreConfigString = AppConfiguration["ConnectionStrings:NpgDevSqlConnection"] ?? string.Empty;
        var container = new ContainerBuilder();
        
        AutoFac.RegisterMapper(container);
        var mapper = AutoFac.Resolve<IMapper>();
        
        // Настройка тестовых контекстов.
        var optionsBuilder = new DbContextOptionsBuilder<PgContext>();
        optionsBuilder.UseNpgsql(PostgreConfigString);
        PgContext = new PgContext(optionsBuilder.Options);

        LogService = new LogService(PgContext);
        UserRepository = new UserRepository(PgContext);
        UserService = new UserService(LogService, UserRepository, mapper);
    }
}