using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CSIT_314_Group.DTO.UserDTO;

namespace CSIT_314_Group.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserAccountRepository _userAccountRepository;
        public AuthController(UserAccountRepository userAccountRepository)
        {
            _userAccountRepository = userAccountRepository;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var user = await _userAccountRepository.GetByEmail(loginDto.email.ToLower());

            if (user == null)
            {
                return Unauthorized("invalid email or password");
            }
            var hasher = new PasswordHasher<Entity.UserAccount>();
            var verifyPassword = hasher.VerifyHashedPassword(user, user.HashedPassword, loginDto.password);
            if (verifyPassword == PasswordVerificationResult.Failed)
            {
                return Unauthorized("invalid email or password");
            }
            if( user.IsSuspended == true)
            {
                return Unauthorized("User suspended");
            }

            var claims = new List<Claim>{
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Profile)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                IsPersistent = false
            });

            return Ok("Logged in");
        }

        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("Logged out");
        }
    }
}
