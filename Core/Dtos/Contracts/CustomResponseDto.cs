using System.Text.Json.Serialization;

namespace Core.Dtos.Contracts
{
    public class CustomResponseDto<T>
    {
        public T? Data { get; set; }

        [JsonIgnore]
        public int StatusCode { get; set; }
        public List<string> Errors { get; set; }

        public static CustomResponseDto<T> Success(T data,int statusCode = 200){
            return new CustomResponseDto<T> { Data = data,StatusCode = statusCode };
        }

        public static CustomResponseDto<T> Success(int statusCode = 200){
            return new CustomResponseDto<T> {StatusCode = statusCode };
        }

        public static CustomResponseDto<T> Fail(List<string> errors,int statusCode = 400){
            return new CustomResponseDto<T> {StatusCode = statusCode,Errors = errors };
        }

        public static CustomResponseDto<T> Fail(string error,int statusCode = 400){
            return new CustomResponseDto<T> {StatusCode = statusCode,Errors = new List<string>{error} };
        }


    }
}