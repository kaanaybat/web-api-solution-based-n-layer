using System.Reflection;
using Api.Filters;
using AspNetCoreRateLimit;
using Data.Context;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Mapping;
using Service.Validations;

namespace Api.Installers.Service
{
    public static class FilterInstaller
    {
        public static IServiceCollection RegisterFilterInstaller(this IServiceCollection services){
            
            services.AddScoped(typeof(NotFoundFilter<>));
            services.AddScoped(typeof(ApiKeyAuthAttribute));

            return services;
        }
    }
}