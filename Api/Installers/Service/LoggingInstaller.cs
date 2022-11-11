using Logging;
using Logging.Configurations.ColumnWriters;
using Microsoft.AspNetCore.HttpLogging;
using NpgsqlTypes;
using Serilog;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.PostgreSQL;

namespace Api.Installers.Service
{
    public static class LoggingInstaller
    {
         public static IServiceCollection RegisterLoggingInstaller(this IServiceCollection services){

            services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = HttpLoggingFields.All;
                logging.RequestHeaders.Add("sec-ch-ua");
                logging.MediaTypeOptions.AddText("application/javascript");
                logging.RequestBodyLogLimit = 4096;
                logging.ResponseBodyLogLimit = 4096;
            });

            return services;
        }
    }
}