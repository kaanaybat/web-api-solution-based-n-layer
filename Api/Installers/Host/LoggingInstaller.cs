using Logging;
using Logging.Configurations.ColumnWriters;
using NpgsqlTypes;
using Serilog;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.PostgreSQL;

namespace Api.Installers.Host
{
    public static class LoggingInstaller
    {
         public static ConfigureHostBuilder RegisterLoggingInstaller(this ConfigureHostBuilder builder,IConfiguration configuration){

            Serilog.ILogger log = new LoggerConfiguration()
                        .Enrich.FromLogContext() // This is required for extending context properties
                        .Enrich.WithMachineName()
                        .Enrich.WithProperty("Application", "Lead Web API")
                        .MinimumLevel.Information()
                        // .Filter.ByExcluding(Matching.FromSource("Microsoft")) // only set logger by manuel
                        // .Filter.ByExcluding(Matching.FromSource("System")) // only set logger by manuel
                        .WriteTo.Console(new CustomTextFormatter())
                        .WriteTo.File("Logs/logs.txt")
                        .WriteTo.PostgreSQL(configuration.GetConnectionString("PostgreSqlConnection"),"Logs",
                        needAutoCreateTable:true,
                        columnOptions: new Dictionary<string, ColumnWriterBase>
                            {
                                {"message", new RenderedMessageColumnWriter(NpgsqlDbType.Text)},
                                {"message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text)},
                                {"level", new LevelColumnWriter(true , NpgsqlDbType.Varchar)},
                                {"time_stamp", new TimestampColumnWriter(NpgsqlDbType.Timestamp)},
                                {"exception", new ExceptionColumnWriter(NpgsqlDbType.Text)},
                                {"log_event", new LogEventSerializedColumnWriter(NpgsqlDbType.Json)},
                                {"user_name", new UserNameColumnWriter()}
                            })
                        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"])) // for the docker-compose implementation
                                    {
                                        CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
                                        TypeName = null,
                                        AutoRegisterTemplate = true,
                                        IndexFormat = "lead-app-logs-{0:yyyy-MM-dd}",
                                    })
                        .CreateLogger();

            builder.UseSerilog(log);

            return builder;
        }
    }
}