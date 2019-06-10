using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestApiTest.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController
    {
        private IBlogPostRepository repository;
        ILogger<BlogController> logger;

        public CommentController(ILogger<BlogController> log, IBlogPostRepository repository)
        {
            logger = log;
            this.repository = repository;
        }
    }
}
