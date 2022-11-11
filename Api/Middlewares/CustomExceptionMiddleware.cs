using System.Net.Mime;
using System.Text.Json;
using Core.Dtos.Contracts;
using Microsoft.AspNetCore.Diagnostics;
using Service.Exceptions;

namespace Api.Middlewares
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (System.Exception)
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

                var response = CustomResponseDto<NoContentDto>.Fail(exceptionFeature.Error.Message,statusCode);

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }

        }

    }

    public static class CustomExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionMiddleware(this IApplicationBuilder builder)
        {

            return builder.UseMiddleware<CustomExceptionMiddleware>();
                
        }
    } 
}