using Asp.Versioning;
using FluentValidation;
using Inv.Api.Middlewares;
using Inv.Api.Profiles;
using Inv.Application;
using Inv.Application.Base;
using Inv.Application.Contracts.Injections;
using Inv.Application.Contracts.Persistence;
using Inv.Application.Contracts.Security;
using Inv.Domain.Settings;
using Inv.Infrastructure.Database.Context;
using Inv.Infrastructure.Repositories;
using Inv.Infrastructure.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Threading.RateLimiting;

namespace Inv.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration) => _configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.Configure<AuthSettings>(options => _configuration.GetSection("Auth").Bind(options));

            var conn = _configuration.GetConnectionString("Main");
            services.AddDbContext<AppDbContext>(o => o.UseSqlServer(conn));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.Scan(scan =>
                scan.FromApplicationDependencies(assembly =>
                        assembly.GetName().Name!.StartsWith("Inv", StringComparison.Ordinal))
                    .AddClasses(classes =>
                        classes.AssignableTo(typeof(IValidator<>))
                            .Where(t => t.BaseType?.IsGenericType == true))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime());

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<HandlerBase>());

            services.Scan(scan =>
                scan.FromAssembliesOf(typeof(Repository<,,>))
                    .AddClasses(classes => classes.AssignableTo(typeof(IRepository<,>)))
                    .AsMatchingInterface()
                    .WithScopedLifetime());


            #region Authentication

            var authSetting = _configuration.GetSection("Auth").Get<AuthSettings>();

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.MapInboundClaims =
                    false; //Clears "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/" from the claims
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidIssuer = authSetting.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSetting.Secret))
                };
            });

            #endregion

            services.AddApiVersioning(o =>
            {
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.ReportApiVersions = true;
            });

            services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Inv.API",
                    Version = "v1",
                    Description = "Inv API"
                });
                o.CustomSchemaIds(t => t.FullName);
                o.EnableAnnotations();
            });

            #region AutoMapper

            //AutoMapper could also use Scrutor with scan.
            services.AddAutoMapper(typeof(AuthProfile).Assembly);
            services.AddAutoMapper(typeof(WarehouseProfile).Assembly);
            services.AddAutoMapper(typeof(Application.Warehouses.Profiles.WarehouseProfile).Assembly);

            #endregion

            #region Dynamic Scopes

            //This scans for dynamically implemented injection scope instances
            services.Scan(scan => scan
                .FromApplicationDependencies(assembly =>
                    assembly.GetName().Name!.StartsWith("Inv", StringComparison.Ordinal))

                .AddClasses(classes => classes.AssignableTo<ISingletonInstance>())
                .AsMatchingInterface()
                .WithSingletonLifetime()

                .AddClasses(classes => classes.AssignableTo<IScopedInstance>())
                .AsMatchingInterface()
                .WithScopedLifetime()

                .AddClasses(classes => classes.AssignableTo<ITransientInstance>())
                .AsMatchingInterface()
                .WithTransientLifetime()
            );

            #endregion

            #region Policies

            var defaultWindow = TimeSpan.FromMinutes(1);
            services.AddRateLimiter(options =>
            {
                options.AddPolicy(AppPolicies.Default, context =>
                {
                    var key = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = defaultWindow,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    });
                });
                options.AddPolicy(AppPolicies.Login, context =>
                {
                    var key = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
                    {
                        //Limit to 5 requests per minute per IP
                        PermitLimit = 5,
                        Window = defaultWindow,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    });
                });
            });

            #endregion
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting()
                .UseErrorHandler()
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(e => e.MapControllers());


            if (env.EnvironmentName != "Integration")
            {
                using var scope = app.ApplicationServices.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                context.Database.Migrate();

                #region Only For Interview Testing Purposes

                //Just to ensure idempotency. I just couldn't find a short way to make this better in given time
                if (!context.Users.Any())
                {
                    SeedMachine.Seed(context, scope.ServiceProvider.GetService<IPasswordHasher>());
                }

                #endregion
            }

            //else if (!env.IsProduction()) //Activate later
            //{
            app.UseSwagger();
            app.UseSwaggerUI();
            //}
        }
    }
}