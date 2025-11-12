using System.Net;
using Inv.Domain.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Inv.Api.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _env;

        public ErrorHandlerMiddleware(RequestDelegate next, IHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                string result;

                //This part could be much better in terms of creating a solid and a standart class
                switch (error)
                {
                    case FluentValidation.ValidationException e:
                        result = $"{e.Message} | {string.Join(",", e.Errors.Select(c => $"{c.PropertyName} | {c.ErrorMessage}"))}";
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;

                    case BusinessException e:
                        result = $"{e.ErrorCode} | {e.Message}";
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;

                    default:
                        result = !_env.IsProduction() 
                            ? error.Message //We don't want to leak any info
                            : "Unknown error occured!";
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                }

                await response.WriteAsync(result);
            }
        }
    }

    public static class ErrorHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlerMiddleware>();
        }
    }
}