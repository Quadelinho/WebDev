﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;

namespace RestApiTest.Controllers
{

    [Route("api/[controller]")] //TODO: naprawić token w blog
    [ApiController]
    public class ErrorController : ControllerBase
    {
        private readonly IHostingEnvironment _environment;
        public ErrorController(IHostingEnvironment env)
        {
            _environment = env;
        }
        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)] //Wyklucza z automatycznego przechodzenia przez framework (w refleksji)
        public IActionResult Index()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionFeature != null)
            {
                Exception exceptionThatOccurred = exceptionFeature.Error;
                string routeWhereExceptionOccurred = exceptionFeature.Path;
                if (exceptionThatOccurred.GetType() == null)//typeof(BlogPostsDomainException))
                {
                    var problemDetails = new ValidationProblemDetails()
                    {
                        Instance = routeWhereExceptionOccurred,
                        Status = StatusCodes.Status400BadRequest,
                        Detail = "Please refer to the errors property for additional details."
                    };
                    problemDetails.Errors.Add("DomainValidations", new string[] { exceptionThatOccurred.Message.ToString() });
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