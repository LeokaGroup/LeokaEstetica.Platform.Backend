using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using LeokaEstetica.Platform.Backend.Loaders.Jobs;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Utils;
using LeokaEstetica.Platform.Notifications.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    EnvironmentName = Environments.Development
}); 
var configuration = builder.Configuration;

builder.Services.AddControllers().AddControllersAsServices();
builder.Services.AddCors(options => options.AddPolicy("ApiCorsPolicy", b =>
{
    b.WithOrigins(configuration.GetSection("CorsUrls:Urls").Get<string[]>())
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
}));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<PgContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("NpgDevSqlConnection") ?? string.Empty));
}
      
if (builder.Environment.IsStaging())
{
    builder.Services.AddDbContext<PgContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("NpgTestSqlConnection") ?? string.Empty));
}

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Leoka.Estetica.Platform" });
    AddSwaggerXml(c);
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
    .UseUrls("http://*:9992");

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

builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(AutoFac.Init);

// Нужно для типа timestamp в Postgres.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Подключаем SignalR.
builder.Services.AddSignalR();

// Подключаем кэш Redis.
builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = configuration["Redis:RedisCacheUrl"] ?? string.Empty;
    options.InstanceName = "LeokaEstetica_";
});

// Добавляем Fluent Validation.
builder.Services.AddFluentValidation(conf =>
{
    conf.RegisterValidatorsFromAssembly(typeof(Program).Assembly);
    conf.AutomaticValidationEnabled = false;
});

// Запуск джоб при старте ядра системы.
StartJobs.Start();

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

app.MapHub<NotifyHub>("/notify"); // Добавляем роут для хаба.

app.Run();