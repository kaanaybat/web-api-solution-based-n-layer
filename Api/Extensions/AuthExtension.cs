using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Api.Extensions
{
    public static class AuthExtension
    {
        public static string getUserId(this HttpContext httpContext){

            if(httpContext.User == null){
                return string.Empty;
            }

            return httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}