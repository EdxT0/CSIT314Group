using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CSIT_314_Group.DTO.UserAccountDTO;
using CSIT_314_Group.Entity;

namespace CSIT_314_Group.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserAccountRepository _userAccountRepository;
        private readonly UserProfileRepository _userProfileRepository;
        private readonly PasswordHasher<UserAccount> _hasher;
        public AuthController(UserAccountRepository userAccountRepository, UserProfileRepository userProfileRepository, PasswordHasher<UserAccount> hasher)
        {
            _userAccountRepository = userAccountRepository;
            _userProfileRepository = userProfileRepository;
            _hasher = hasher;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            Console.WriteLine($"Login attempt: {loginDto.Email}");

            var user = await _userAccountRepository.GetByEmail(loginDto.Email.ToLower());
            Console.WriteLine($"User found: {user != null}");

            if (user == null)
                return Unauthorized("invalid email or password");

            var verifyPassword = _hasher.VerifyHashedPassword(user, user.HashedPassword, loginDto.Password);
            Console.WriteLine($"Password verify result: {verifyPassword}");

            if (verifyPassword == PasswordVerificationResult.Failed)
                return Unauthorized("invalid email or password");

            Console.WriteLine($"IsSuspended: {user.IsSuspended}");
            if (user.IsSuspended == true)
                return Unauthorized("User suspended");

            Console.WriteLine($"ProfileId: {user.ProfileId}");
            string profileName = await _userProfileRepository.getProfileNameWithId(user.ProfileId);
            Console.WriteLine($"ProfileName: {profileName}");

            // ↓ YOUR ORIGINAL CODE STAYS HERE — everything below is unchanged
            if (await _userProfileRepository.IsProfileSuspended(user.ProfileId))
            {
                return Unauthorized($"Profile {profileName} is suspended");
            }

            var claims = new List<Claim>{
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Name),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, profileName.ToLower())
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                IsPersistent = false
            });

            return Ok(new
            {
                message = "Logged in",
                user = new
                {
                    id = user.Id,
                    name = user.Name,
                    email = user.Email,
                    role = await _userProfileRepository.getProfileNameWithId(user.ProfileId)
                }
            });
        }
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("Logged out");
        }
    }
}
