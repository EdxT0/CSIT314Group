using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CSIT_314_Group.DTO.UserAccountDTO;

namespace CSIT_314_Group.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly Data.UserAccount _userAccountRepository;
        private readonly Data.UserProfile _userProfileRepository;
        private readonly PasswordHasher<UserAccount> _hasher;
        public AuthController(Data.UserAccount userAccountRepository, Data.UserProfile userProfileRepository, PasswordHasher<UserAccount> hasher)
        {
            _userAccountRepository = userAccountRepository;
            _userProfileRepository = userProfileRepository;
            _hasher = hasher;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {

            var user = await _userAccountRepository.GetByEmail(loginDto.Email.ToLower());

            if (user == null)
                return Unauthorized("invalid email or password");

            var verifyPassword = _hasher.VerifyHashedPassword(user, user.HashedPassword, loginDto.Password);

            if (verifyPassword == PasswordVerificationResult.Failed)
                return Unauthorized("invalid email or password");

            if (user.IsSuspended == true)
                return Unauthorized("User suspended");

            string profileName = await _userProfileRepository.getProfileNameWithId(user.ProfileId);

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

        //Testing to see if refreshing keeps the user logged in
        [HttpGet("Me")]
        public IActionResult Me()
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            return Ok(new
            {
                id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                name = User.FindFirst(ClaimTypes.Name)?.Value,
                email = User.FindFirst(ClaimTypes.Email)?.Value,
                role = User.FindFirst(ClaimTypes.Role)?.Value,
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
