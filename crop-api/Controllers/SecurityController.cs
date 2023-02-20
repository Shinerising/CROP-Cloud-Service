using CROP.API.Data;
using CROP.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CROP.API.Controllers
{
    /// <summary>
    /// Controller for security.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api")]
    public class SecurityController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly PostgresDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityController"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="context">The context.</param>
        public SecurityController(IConfiguration configuration, PostgresDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        /// <summary>
        /// Creates a new token.
        /// </summary>
        /// <param name="user">The user to create the token for.</param>
        [AllowAnonymous]
        [HttpPost("security/login", Name = "CreateToken")]
        public ActionResult<TokenData> Get([FromBody] UserInput user)
        {
            var result = _context.Users.First(_user => user.UserName == _user.UserName);
            if (result == null)
            {
                return Unauthorized();
            }

            var hasher = new PasswordHasher<UserData>();
            if (hasher.VerifyHashedPassword(result, result.Password, user.Password) == PasswordVerificationResult.Failed)
            {
                return Unauthorized();
            }

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                        new Claim("Id", result.Id.ToString()),
                        new Claim(ClaimTypes.Role, result.Role),
                        new Claim(JwtRegisteredClaimNames.Sub, result.UserName),
                        new Claim(JwtRegisteredClaimNames.Email, result.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    }),
                Expires = DateTime.UtcNow.AddMinutes(60),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);
            return Ok(new TokenData(jwtToken, token.ValidTo));
        }

        [Authorize]
        [HttpPost("security/touch", Name = "Touch")]
        public ActionResult Touch()
        {
            return Ok();
        }
    }
    
    /// <summary>
    /// The user input.
    /// </summary>
    public record TokenData(string Token, DateTime ExpiredTime);
}
