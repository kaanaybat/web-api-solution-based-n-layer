using System.Net;
using System.Text;
using Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace Api.Filters
{
    public class CacheAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _absoluteTime; 
        private readonly bool _isUserBased;    
         private readonly string _key = "";        
    
        public CacheAttribute(int absoluteTime,bool isUserBased = false)
        {
            _absoluteTime = absoluteTime;
            _isUserBased = isUserBased;
        }

        public CacheAttribute(int absoluteTime,string key,bool isUserBased = false)
        {
            _absoluteTime = absoluteTime;
            _isUserBased = isUserBased;
            _key = key;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var inMemoryService = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();

            var userId = _isUserBased == true ? context.HttpContext.getUserId() : "";
            var cacheKey = _key == "" ? GenerateCacheKey(context.HttpContext.Request,userId) : _key;

            var cacheResponse =  inMemoryService.Get<string>(cacheKey);

            if(!string.IsNullOrEmpty(cacheResponse)){

                var contentResult = new ContentResult
                {
                        Content = cacheResponse,
                        ContentType = "application/json",
                        StatusCode = 200
                };
                context.Result = contentResult;
                return;
            }

            var executedContext = await next();

            if((executedContext.Result is ObjectResult ObjectResult) && executedContext.HttpContext.Response.StatusCode == (int)HttpStatusCode.OK ){

                var options = new MemoryCacheEntryOptions(){
                    AbsoluteExpiration = DateTime.Now.AddSeconds(_absoluteTime)
                };

                var json = JsonSerializer.Serialize(ObjectResult.Value,new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true    
                });
                    
                inMemoryService.Set<string>(cacheKey,json,options);
            }
            
        }
    
        private static string GenerateCacheKey(HttpRequest request, string userId = null){

            var keyBuilder = new StringBuilder();

            keyBuilder.Append($"{request.Path}");

            foreach (var item in request.Query.OrderBy(x => x.Key))
            {
                keyBuilder.Append($"|{item.Key}-{item.Value}");
            }
            
            if (!string.IsNullOrEmpty(userId)) {
                keyBuilder.Append(userId);
            }

            return keyBuilder.ToString();
        }
    
    }
}  