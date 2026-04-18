using CSIT_314_Group.Data;
using CSIT_314_Group.Entity;
using CSIT_314_Group.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using CSIT_314_Group.DTO.UserDTO;


namespace CSIT_314_Group.Controllers.UserAccount
{
    [ApiController]
    [Route("api/[controller]")]
    public class CreateUserAccountController : ControllerBase
    {
        private readonly UserAccountRepository _userAccountRepository;

        public CreateUserAccountController(UserAccountRepository userAccountRepository)
        {
            _userAccountRepository = userAccountRepository;
        }

        [Authorize(Roles ="admin")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO createUserRequest)
        {
            string profileInputLowerCase = createUserRequest.Profile.ToLower();
            if(profileInputLowerCase != "admin" && profileInputLowerCase != "donee" && profileInputLowerCase != "fundraiser" && profileInputLowerCase != "platform manager")
            {
                return BadRequest($"Invalid Profile {createUserRequest.Profile}");
            }
                var hasher = new PasswordHasher<Entity.UserAccount>();
            var userDetails = new Entity.UserAccount
            {
                Name = createUserRequest.Name.ToLower(),
                Email = createUserRequest.Email.ToLower(),
                PhoneNumber = createUserRequest.PhoneNumber.ToLower(),
                Profile = profileInputLowerCase,
                IsSuspended = createUserRequest.IsSuspended
            };
            userDetails.HashedPassword = hasher.HashPassword(userDetails, createUserRequest.Password);

            CreateUserResultEnum result = await _userAccountRepository.CreateUser(userDetails);
            return result switch
            {
                CreateUserResultEnum.Success => Ok($"User {userDetails.Name} Created"),
                CreateUserResultEnum.DuplicateEmail => Conflict("Email already exists"),
                CreateUserResultEnum.DuplicatePhoneNumber => Conflict("Phone number already exists"),
                CreateUserResultEnum.DuplicatePhoneNumberAndEmail => Conflict("Email or phone number already exist."),
                CreateUserResultEnum.Failed => Problem("unable to create user, please try again"),
                _ => StatusCode(500, "Could not create user.")
            };
        }

    }
}
