using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using Hellang.Middleware.ProblemDetails;
using LeokaEstetica.Platform.Access.AutofacModules;
using LeokaEstetica.Platform.Backend.Loaders.Bots;
using LeokaEstetica.Platform.Backend.Loaders.Jobs;
using LeokaEstetica.Platform.Base.AutofacModules;
using LeokaEstetica.Platform.Base.Factors;
using LeokaEstetica.Platform.CallCenter.AutofacModules;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Utils;
using LeokaEstetica.Platform.Database.AutofacModules;
using LeokaEstetica.Platform.Diagnostics.AutofacModules;
using LeokaEstetica.Platform.Finder.AutofacModules;
using LeokaEstetica.Platform.Integrations.AutofacModules;
using LeokaEstetica.Platform.Integrations.Filters;
using LeokaEstetica.Platform.Messaging.AutofacModules;
using LeokaEstetica.Platform.Notifications.AutofacModules;
using LeokaEstetica.Platform.Notifications.Data;
using LeokaEstetica.Platform.Processing.AutofacModules;
using LeokaEstetica.Platform.Redis.AutofacModules;
using LeokaEstetica.Platform.Services.AutofacModules;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;
using Quartz;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers(opt =>
    {
        opt.Filters.Add(typeof(PachcaLogExceptionFilter));
    })
    .AddControllersAsServices()
    .AddNewtonsoftJson();

builder.Services.AddCors(options => options.AddPolicy("ApiCorsPolicy", b =>
{
    b.WithOrigins(configuration.GetSection("CorsUrls:Urls").Get<string[]>())
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
}));

builder.Environment.EnvironmentName = configuration["Environment"];

builder.Services.AddHttpContextAccessor();

string connection = null;

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<PgContext>(options =>
            options.UseNpgsql(configuration["ConnectionStrings:NpgDevSqlConnection"]),
        ServiceLifetime.Transient);
    connection = configuration["ConnectionStrings:NpgDevSqlConnection"];
}

if (builder.Environment.IsStaging())
{
    builder.Services.AddDbContext<PgContext>(options =>
            options.UseNpgsql(configuration["ConnectionStrings:NpgTestSqlConnection"]),
        ServiceLifetime.Transient);
    connection = configuration["ConnectionStrings:NpgTestSqlConnection"];
}

if (builder.Environment.IsProduction())
{
    builder.Services.AddDbContext<PgContext>(options =>
            options.UseNpgsql(configuration["ConnectionStrings:NpgSqlConnection"]),
        ServiceLifetime.Transient);
    connection = configuration["ConnectionStrings:NpgSqlConnection"];
}

// builder.Services.AddTransient<IConnectionFactory>(_ => new NpgSqlConnectionFactory(connection));

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Leoka.Estetica.Platform" });
    AddSwaggerXml(c);
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Введите валидный токен.",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Добавляем xml-комментарии для всех API.
static void AddSwaggerXml(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions c)
{
    var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml");
    foreach (var xmlFile in xmlFiles)
    {
        c.IncludeXmlComments(xmlFile);
    }
}

builder.WebHost
    .UseKestrel()
    .UseContentRoot(Directory.GetCurrentDirectory())
    .UseUrls(configuration["UseUrls:Path"])
    .UseEnvironment(configuration["Environment"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,
            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,
            ValidateLifetime = true,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true
        };
    });

// builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
//     .ConfigureContainer<ContainerBuilder>(AutoFac.Init);
// builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
//     .ConfigureContainer<ContainerBuilder>(b =>
//     {
//         // b.RegisterModule(new RepositoriesModule());
//         // b.RegisterModule(new ServicesModule());
//         // b.RegisterModule(new CallCenterModule());
//         // b.RegisterModule(new ProcessingModule());
//         // b.RegisterModule(new AccessModule());
//         // b.RegisterModule(new MetricsModule());
//         // b.RegisterModule(new FinderModule());
//         // b.RegisterModule(new IntegrationModule());
//         // b.RegisterModule(new MessagingModule());
//         // b.RegisterModule(new NotificationsModule());
//         // b.RegisterModule(new RedisModule());
//         // b.RegisterModule(new BaseModule());
//         
//         RepositoriesModule.InitModules(b);
//         ServicesModule.InitModules(b);
//         CallCenterModule.InitModules(b);
//         ProcessingModule.InitModules(b);
//         AccessModule.InitModules(b);
//         MetricsModule.InitModules(b);
//         FinderModule.InitModules(b);
//         IntegrationModule.InitModules(b);
//         MessagingModule.InitModules(b);
//         NotificationsModule.InitModules(b);
//         RedisModule.InitModules(b);
//         BaseModule.InitModules(b);
//     });

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(b =>
    {
        // b.RegisterModule(new RepositoriesModule());
        // b.RegisterModule(new ServicesModule());
        // b.RegisterModule(new CallCenterModule());
        // b.RegisterModule(new ProcessingModule());
        // b.RegisterModule(new AccessModule());
        // b.RegisterModule(new MetricsModule());
        // b.RegisterModule(new FinderModule());
        // b.RegisterModule(new IntegrationModule());
        // b.RegisterModule(new MessagingModule());
        // b.RegisterModule(new NotificationsModule());
        // b.RegisterModule(new RedisModule());
        // b.RegisterModule(new BaseModule());

        // RepositoriesModule.InitModules(b);
        // ServicesModule.InitModules(b);
        // CallCenterModule.InitModules(b);
        // ProcessingModule.InitModules(b);
        // AccessModule.InitModules(b);
        // MetricsModule.InitModules(b);
        // FinderModule.InitModules(b);
        // IntegrationModule.InitModules(b);
        // MessagingModule.InitModules(b);
        // NotificationsModule.InitModules(b);
        // RedisModule.InitModules(b);
        // BaseModule.InitModules(b);

        AutoFac.Init(b);
        
        b.RegisterType<NpgSqlConnectionFactory>()
            .As<IConnectionFactory>()
            .WithParameter("connectionString", connection)
            .InstancePerLifetimeScope();
    });

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Подключаем SignalR.
builder.Services.AddSignalR();

// Подключаем кэш Redis.
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration["Redis:RedisCacheUrl"] ?? string.Empty;
});

// Добавляем Fluent Validation.
builder.Services.AddFluentValidation(conf =>
{
    conf.RegisterValidatorsFromAssembly(typeof(Program).Assembly);
    conf.AutomaticValidationEnabled = false;
});

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    // Запуск джоб при старте ядра системы.
    StartJobs.Start(q, builder.Services);
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

builder.Logging.ClearProviders();
builder.Host.UseNLog();

builder.Services.AddProblemDetails();

// Запускаем ботов.
await LogNotifyBot.RunAsync(configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseCors("ApiCorsPolicy");
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

if (builder.Environment.IsDevelopment() || builder.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Leoka.Estetica.Platform"));
}

// Добавляем хаб приложения для работы через сокеты.
app.MapHub<ChatHub>("/notify");

app.UseProblemDetails();

app.Run();