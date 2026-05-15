using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CSIT_314_Group.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserAccount _userAccountRepository;
        private readonly UserProfile _userProfileRepository;
        private readonly PasswordHasher<UserAccount> _hasher;
        public AuthController(Data.UserAccount userAccountRepository, Data.UserProfile userProfileRepository, PasswordHasher<UserAccount> hasher)
        {
            _userAccountRepository = userAccountRepository;
            _userProfileRepository = userProfileRepository;
            _hasher = hasher;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(
            [FromQuery] string email,
            [FromBody] string password
            )
        {

            var success = await _userAccountRepository.Login(email, password);
            if (success)
            {
                var user = await _userAccountRepository.GetByEmail(email);

                var claims = new List<Claim>{
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.ProfileName?.ToLower() ?? "")
                 };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                {
                    IsPersistent = false
                });

                return Ok(true);
            }

            return BadRequest(false);

        }

        [HttpGet("Me")]
        public IActionResult Me()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return Unauthorized();

            var role = User.FindFirstValue(ClaimTypes.Role);
            var name = User.FindFirstValue(ClaimTypes.Name);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Ok(new { id, name, email, role });
        }



        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("Logged out");
        }


    }
}