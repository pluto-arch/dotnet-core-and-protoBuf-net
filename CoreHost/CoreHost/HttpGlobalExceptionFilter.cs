using System;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using ILogger = Serilog.ILogger;

namespace CoreHost
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IHostingEnvironment _env;
        public readonly ILogger<HttpGlobalExceptionFilter> _log;

        public HttpGlobalExceptionFilter(ILogger<HttpGlobalExceptionFilter> log, IHostingEnvironment env)
        {
            _log = log;
            _env = env;
        }


        public void OnException(ExceptionContext context)
        {
            _log.LogError(new EventId(context.Exception.HResult),
                context.Exception,
                context.Exception.Message);
            if (context.Exception.GetType() == typeof(CatalogDomainException))
            {
                var problemDetails = new ValidationProblemDetails()
                {
                    Instance = context.HttpContext.Request.Path,
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Please refer to the errors property for additional details."
                };

                problemDetails.Errors.Add("DomainValidations", new string[] { context.Exception.Message.ToString() });

                context.Result = new BadRequestObjectResult(problemDetails);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else
            {
                var json = new JsonErrorResponse
                {
                    Messages = new[] { "An error ocurred." }
                };

                if (_env.IsDevelopment())
                {
                    json.DeveloperMessage = context.Exception;
                }

                context.Result = new InternalServerErrorObjectResult(json);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            context.ExceptionHandled = true;
        }
        private class JsonErrorResponse
        {
            public string[] Messages { get; set; }

            public object DeveloperMessage { get; set; }
        }

        private class InternalServerErrorObjectResult:ObjectResult
        {
            public InternalServerErrorObjectResult(object error)
                : base(error)
            {
                StatusCode = StatusCodes.Status500InternalServerError;
            }
        }

        public class CatalogDomainException : Exception
        {
            public CatalogDomainException()
            { }

            public CatalogDomainException(string message)
                : base(message)
            { }

            public CatalogDomainException(string message, Exception innerException)
                : base(message, innerException)
            { }
        }
    }
}