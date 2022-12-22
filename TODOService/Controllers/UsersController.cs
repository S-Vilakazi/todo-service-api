using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TODOService.Models;
using TODOService.Tools;
namespace TODOService.Controllers
{
    [EnableCors("appCors")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly TodoAppDatabaseContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(TodoAppDatabaseContext _context, IConfiguration _configuration)
        {
            this._context = _context;
            this._configuration = _configuration;
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> userLogin([FromBody] User user)
        {
            try
            {
                String password = Password.hashPassword(user.Password);
                var dbUser = _context.Users.Where(u => u.Username == user.Username && u.Password == password).Select(u => new
                {
                    u.UserId,
                    u.Username,
                    u.Active
                }).FirstOrDefault();

                if (dbUser == null)
                {
                    return BadRequest("Username or password is incorrect");
                }

                List<Claim> autClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, dbUser.Username),
                    new Claim("userID",dbUser.UserId.ToString()),
                     new Claim("username",dbUser.Username)
                };

                var token = this.getToken(autClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });

            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
           
        }
        
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> userRegisteration([FromBody] User user)
        {

            try
            {
                var dbUser = _context.Users.Where(u => u.Username == user.Username).FirstOrDefault();
                if (dbUser != null)
                {
                    return BadRequest("Username already exists");

                }

                user.Password = Password.hashPassword(user.Password);
                user.Active = 1;
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok(new { message="User is successfully registered" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        private JwtSecurityToken getToken(List<Claim> authClaim)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));


            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(24),
                claims : authClaim,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return token;
        }
       
    
    }
}
