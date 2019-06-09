using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestApiTest.Core.Exceptions;
using System;
using System.Net;

namespace RestApiTest.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        //[Note] Zazwyczaj taka forma kontrolera błędów jest wystarczająca dla większości aplikacji webowych
        private readonly IHostingEnvironment _environment;
        private ILogger<ErrorController> logger;

        public ErrorController(ILogger<ErrorController> log, IHostingEnvironment env)
        {
            _environment = env;
            logger = log;
        }
        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)] //[Note] Wyklucza z automatycznego przechodzenia przez framework (w refleksji)
        public IActionResult Index()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionFeature != null)
            {
                Exception exceptionThatOccurred = exceptionFeature.Error;
                string routeWhereExceptionOccurred = exceptionFeature.Path;
                if (exceptionThatOccurred.GetType() == typeof(BlogPostsDomainException))
                {
                    var problemDetails = new ValidationProblemDetails()
                    {
                        Instance = routeWhereExceptionOccurred,
                        Status = StatusCodes.Status400BadRequest,
                        Detail = "Please refer to the errors property for additional details."
                    };
                    problemDetails.Errors.Add("DomainValidations", new string[] { exceptionThatOccurred.Message.ToString() });
                    logger.LogError("Domain error details: {@0}", problemDetails);
                    return BadRequest(problemDetails);
                }
                else
                {
                    var problemDetails = new
                    {
                        Error = exceptionThatOccurred.Message,
                        Status = StatusCodes.Status500InternalServerError,
                        Details = _environment.IsDevelopment() ? exceptionThatOccurred.ToString() : null,
                        Instance = routeWhereExceptionOccurred
                    };
                    logger.LogError("Server error details: {@0}", problemDetails);
                    return StatusCode((int)HttpStatusCode.InternalServerError, problemDetails);
                }
            }
            else
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Error = "Unknown error" });
            }
        }
    }
}