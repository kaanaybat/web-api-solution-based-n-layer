using System.Reflection;
using Api.Filters;
using Api.Settings;
using AspNetCoreRateLimit;
using Data.Context;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Mapping;
using Service.Validations;

namespace Api.Installers.Service
{
    public static class ServiceInstaller
    {
        public static IServiceCollection RegisterServiceInstaller(this IServiceCollection services,IConfiguration configuration){
            
            services
            .AddControllers(options =>  options.Filters.Add(new ValidateFilterAttribute()))
            .AddFluentValidation(options => {

                options.RegisterValidatorsFromAssemblyContaining<ProjectCreateDtoValidator>();

            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;

            });

            // configure strongly typed settings object
            services.Configure<CachingKeysSettings>(configuration.GetSection("CachingKeys"));

            services.AddMemoryCache();
            services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
            services.AddInMemoryRateLimiting();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();


            services.AddScoped(typeof(NotFoundFilter<>));
            services.AddScoped(typeof(ApiKeyAuthAttribute));
            services.AddAutoMapper(typeof(MapProfile));

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PostgreSqlConnection"), option => 
                {

                    option.MigrationsAssembly(Assembly.GetAssembly(typeof(AppDbContext)).GetName().Name);

                })
            );

            return services;
        }
    }
}