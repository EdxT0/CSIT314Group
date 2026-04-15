using CSIT_314_Group.Data;
using CSIT_314_Group.DTO;
using CSIT_314_Group.Entity;
using CSIT_314_Group.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace CSIT_314_Group.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserAccountController : ControllerBase
    {
        private readonly UserAccountRepository _userAccountRepository;

        public UserAccountController(UserAccountRepository userAccountRepository)
        {
            _userAccountRepository = userAccountRepository;
        }

        [Authorize(Roles ="admin")]
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO createUserRequest)
        {
            var hasher = new PasswordHasher<UserAccountEntity>();
            var userDetails = new UserAccountEntity
            {
                Name = createUserRequest.Name,
                Email = createUserRequest.Email,
                PhoneNumber = createUserRequest.PhoneNumber,
                Role = createUserRequest.Role
            };
            userDetails.HashedPassword = hasher.HashPassword(userDetails, createUserRequest.Password);

            CreateUserResultEnum result = await _userAccountRepository.CreateUser(userDetails);
            return result switch
            {
                CreateUserResultEnum.Success => Ok("User Created"),
                CreateUserResultEnum.DuplicateEmail => Conflict("Email already exists"),
                CreateUserResultEnum.DuplicatePhoneNumber => Conflict("Phone number already exists"),
                CreateUserResultEnum.DuplicatePhoneNumberAndEmail => Conflict("Email or phone number already exist."),
                CreateUserResultEnum.Failed => Problem("unable to create user, please try again"),
                _ => StatusCode(500, "Could not create user.")
            };
        }

    }
}
