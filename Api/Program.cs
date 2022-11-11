
using Api.Installers.Host;
using Api.Installers.Service;
using Api.Middlewares;
using Api.Modules;
using AspNetCoreRateLimit;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Serilog;
using Serilog.Context;


var builder = WebApplication.CreateBuilder(args);

// **** SERVICE INSTALLER **** //

builder.Services.RegisterServiceInstaller(builder.Configuration);
builder.Services.RegisterFilterInstaller();
builder.Services.RegisterLoggingInstaller();

// **** SERVICE INSTALLER **** //

// ***** AUTO FAC **** //

builder.Host.UseServiceProviderFactory
    (new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder => containerBuilder.RegisterModule(new RepositoryModule()));
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder => containerBuilder.RegisterModule(new ServiceModule()));

// ***** AUTO FAC **** //

// **** HOST INSTALLER **** //

builder.Host.RegisterLoggingInstaller(builder.Configuration);

// **** HOST INSTALLER **** //



var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpLogging();

app.UseHttpsRedirection();
app.UseCustomExceptionHandler(app.Services.GetRequiredService<Serilog.ILogger>());
app.UseAuthorization();

//For logging user information
app.Use(async (context, next) =>
{
    var username = context.User?.Identity?.IsAuthenticated != null || true ? context.User.Identity.Name : null;
    LogContext.PushProperty("user_name", username);
    await next();
});

app.UseIpRateLimiting();
app.MapControllers();
app.Run();

