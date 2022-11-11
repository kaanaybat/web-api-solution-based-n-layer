using System.Net.Mime;
using System.Security.Claims;
using System.Text.Json;
using Core.Dtos.Contracts;
using Microsoft.AspNetCore.Diagnostics;
using Service.Exceptions;

namespace Api.Middlewares
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void UseCustomExceptionHandler(this IApplicationBuilder app,Serilog.ILogger logger)
        {
            app.UseExceptionHandler(config =>
            {

                config.Run(async context =>
                {
                    // context.Response.ContentType = "application/json";
                    context.Response.ContentType = MediaTypeNames.Application.Json;

                    var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();

                    var statusCode = exceptionFeature.Error switch
                    {
                        ClientSideException => 400,
                        NotFoundExcepiton=> 404,
                        _ => 500
                    };
                    context.Response.StatusCode = statusCode;
                
                    logger.Error(
                           "IP:{Ip}," +
                           "UserId:{UserId}," +
                           "Scheme:{Scheme} ," +
                           "Host: {Host} ," +
                           "Method:{Method} ," +
                           "QueryString: {QueryString} ," +
                           "Exception: {Exception} ," +
                           "StatusCode:{StatusCode}",    
                           context.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                           context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                           context.Request.Scheme,
                           context.Request.Host.Host,
                           context.Request.Method,
                           context.Request.QueryString.Value,
                           exceptionFeature.Error.Message,
                           context.Response.StatusCode);


                    var response = CustomResponseDto<NoContentDto>.Fail(exceptionFeature.Error.Message,statusCode);


                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));

                });

            });
        }
    }
}