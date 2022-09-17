using Autofac;
using Autofac.Extensions.DependencyInjection;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers().AddControllersAsServices();
builder.Services.AddCors(options => options.AddPolicy("ApiCorsPolicy", b =>
{
    b.WithOrigins(configuration.GetSection("CorsUrls:Urls").Get<string[]>())
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
}));

#region Для дева.

builder.Services.AddDbContext<PgContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("NpgDevSqlConnection") ?? string.Empty));

#endregion

#region Для теста.

builder.Services.AddDbContext<PgContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("NpgTestSqlConnection") ?? string.Empty));

#endregion

builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "Leoka.Estetica.Platform" }); });

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

// Используем Serilog.
builder.Host.UseSerilog((context, config) =>
    {
        try
        {
            var connString = configuration.GetConnectionString("LogDevConnection") ?? string.Empty;
            config.WriteTo.PostgreSQL(connString, "Logs", needAutoCreateTable: true)
                .MinimumLevel.Information();

            if (!context.HostingEnvironment.IsProduction())
            {
                config.WriteTo.Console()
                    .MinimumLevel.Information();
            }
        }
        
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
   
    // .WriteTo.Seq("http://localhost:9992")
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseCors("ApiCorsPolicy");
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Leoka.Estetica.Platform"));
app.Run();