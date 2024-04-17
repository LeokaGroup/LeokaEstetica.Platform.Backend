using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using Hellang.Middleware.ProblemDetails;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Utils;
using LeokaEstetica.Platform.Integrations.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers(opt =>
    {
        opt.Filters.Add(typeof(DiscordLogExceptionFilter));
    })
    .AddControllersAsServices();
    // .AddNewtonsoftJson();

builder.Services.AddCors(options => options.AddPolicy("ApiCorsPolicy", b =>
{
    b.WithOrigins(configuration.GetSection("CorsUrls:Urls").Get<string[]>())
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
}));

builder.Environment.EnvironmentName = configuration["Environment"];

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

builder.Services.AddHttpContextAccessor();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Leoka.Estetica.Platform.ProjectManagement" });
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
    .UseUrls(configuration["UseUrls:ProjectManagmentPath"])
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

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(b =>
    {
        AutoFac.Init(b);

        // b.RegisterType<NpgSqlConnectionFactory>()
        //     .As<IConnectionFactory>()
        //     .WithParameter("connectionString", connection!)
        //     .InstancePerLifetimeScope();
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

builder.Logging.ClearProviders();
builder.Host.UseNLog();

// builder.Services.AddProblemDetails();

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
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Leoka.Estetica.Platform.ProjectManagement"));
}

// Добавляем хаб приложения для работы через сокеты.
// app.MapHub<ProjectManagementHub>("/project-management-notify");

// app.UseProblemDetails();

app.Run();