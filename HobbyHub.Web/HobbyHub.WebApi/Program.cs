using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using FluentValidation;
using HobbyHub.Application.Infrastructure;
using HobbyHub.Application.Middleware;
using HobbyHub.Database.Infrastructure;
using HobbyHub.UserIdentity.Provider.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using StructureMap;
using Serilog;

namespace HobbyHub.WebApi;

[ExcludeFromCodeCoverage]
public static class Program
{
    public static void Main(string[] args)
    {
        CreateWebHostBuilder(args).Run();
    }

    public static WebApplication CreateWebHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.UseStructureMap();
        ConfigureServices(builder.Services, builder.Configuration);
        
        Log.Logger = new LoggerConfiguration()
            .WriteTo
            .Console()
            .CreateLogger();

        builder.Host.UseSerilog((context, loggerConfiguration) =>
        {
            loggerConfiguration.WriteTo.Console();
            loggerConfiguration.ReadFrom.Configuration(context.Configuration);
        });
        
        var app = builder.Build();
        app.UseSerilogRequestLogging();

        Configure(app);
        
        return app;
    }

    private static void UseStructureMap(this WebApplicationBuilder builder)
    {
        builder.Host.UseServiceProviderFactory(_ =>
            new StructureMapServiceProviderFactory(new ApplicationRegistry(builder.Configuration)));
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                };
            });

        services.Configure<JwtConfig>(configuration.GetSection("Jwt"));
        
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        
        services.AddOpenApiDocument();

        services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling =
                    Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo()
            {
                Version = "v1",
                Title = "HobbyHub WepApi",
                Description = "HobbyHub WepApi"
            });

            c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme()
            {
                Name = "JWT Authentication",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme."
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference()
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    new string[] {}
                }
            });
            
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });
        
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("PostgresDatabase"));
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        
        services.AddCors();
    }

    private static void Configure(IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUi();

        app.UseExceptionHandler();
        
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();  
        app.UseAuthentication();  
        app.UseAuthorization();

        app.UseCors(builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());   
        
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
