using Application.ServiceContracts;
using Common.DTO.Request;
using Common.DTO.Response;
using Infrastructure.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace RestApi.Controllers
{
    [Authorize]
    [Route("api/Admin")]
    [ApiController]
    public class AdminController : BaseController
    {
        private readonly ILogger<AdminController> _logger;
        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }

    }
}
