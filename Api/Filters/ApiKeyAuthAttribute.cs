using Core.Dtos.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filters
{
    public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "x-api-key" ;
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
             if(!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName , out var expectedApiKey)){
                context.Result = new UnauthorizedObjectResult(CustomResponseDto<NoContentDto>.Fail($"Unauthorized",401));
                return;
            }

            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = configuration.GetValue<string>("ApiKey");

            if(!apiKey.Equals(expectedApiKey)){
                context.Result = new UnauthorizedObjectResult(CustomResponseDto<NoContentDto>.Fail($"Unauthorized",401));
                return;
            }

            await next();
        }
    }
}