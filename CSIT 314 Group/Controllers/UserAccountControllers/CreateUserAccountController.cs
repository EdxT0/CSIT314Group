using CSIT_314_Group.Data;
using CSIT_314_Group.Entity;
using CSIT_314_Group.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using CSIT_314_Group.DTO.UserAccountDTO;


namespace CSIT_314_Group.Controllers.UserAccountControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CreateUserAccountController : ControllerBase
    {
        private readonly UserAccountRepository _userAccountRepository;
        private readonly UserProfileRepository _userProfileRepository;


        public CreateUserAccountController(UserAccountRepository userAccountRepository, UserProfileRepository userProfileRepository)
        {
            _userAccountRepository = userAccountRepository;
            _userProfileRepository = userProfileRepository;
        }

        [Authorize(Roles ="admin")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO createUserRequest)
        {
            string profileInputLowerCase = createUserRequest.ProfileName.ToLower();
            int? ProfileId = await _userProfileRepository.getIdWithProfileName(profileInputLowerCase);
            if (ProfileId == null)
            {
                return BadRequest($"Invalid Profile {createUserRequest.ProfileName}"); 
            }
            if((await _userAccountRepository.GetIdsWithNameOrEmailOrPhone(createUserRequest.Email)).Count != 0)
            {
                return Conflict("email already exist");
            }

            if ( (await _userAccountRepository.GetIdsWithNameOrEmailOrPhone(createUserRequest.PhoneNumber)).Count != 0 )

            {
                return Conflict("email already exist");
            }
            var hasher = new PasswordHasher<UserAccount>();

            var userDetails = new UserAccount(createUserRequest.Name.ToLower(), createUserRequest.Email.ToLower(), createUserRequest.PhoneNumber.ToLower(), ProfileId, createUserRequest.IsSuspended);
            {
                userDetails.setPassword(hasher.HashPassword(userDetails, createUserRequest.Password));

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
}
