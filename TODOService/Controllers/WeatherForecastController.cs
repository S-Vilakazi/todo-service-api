using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TODOService.Models;

namespace TODOService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly TodoAppDatabaseContext _dbcontext;
        public WeatherForecastController(TodoAppDatabaseContext _context)
        {
            _dbcontext = _context;
        }
        [Authorize]
        [HttpGet]
        [Route("Users")]
        public async Task<IActionResult> GetUsers()
        {
            return Ok("You have this");
        }
        
        [HttpGet]
        [Route("Users/{id}")]
        public async Task<IActionResult> getUserId(int id)
        {
            return Ok(new { userID = id });
        }

        [Authorize]
        [HttpGet]
        [Route("Users/current")]
        public async Task<IActionResult> getloggedInUserID()
        {
            int id = Convert.ToInt32(HttpContext.User.FindFirstValue("userID"));
            return Ok(new { userID = id });
        }


    }
}