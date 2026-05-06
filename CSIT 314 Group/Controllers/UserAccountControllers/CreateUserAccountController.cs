using CSIT_314_Group.Data;
using CSIT_314_Group.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;


namespace CSIT_314_Group.Controllers.UserAccountControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CreateUserAccountController : ControllerBase
    {
        private readonly Data.UserAccount _userAccountRepository;
        private readonly Data.UserProfile _userProfileRepository;


        public CreateUserAccountController(Data.UserAccount userAccountRepository, Data.UserProfile userProfileRepository)
        {
            _userAccountRepository = userAccountRepository;
            _userProfileRepository = userProfileRepository;
        }

        [Authorize(Roles ="admin")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserAccount createUserRequest)
        {

            if (createUserRequest.ProfileId == null)
            {
                return BadRequest($"Invalid Profile"); 
            }
            if((await _userAccountRepository.GetIdsWithNameOrEmailOrPhone(createUserRequest.Email)).Count != 0)
            {
                return Conflict("email already exist");
            }

            if ( (await _userAccountRepository.GetIdsWithNameOrEmailOrPhone(createUserRequest.PhoneNumber)).Count != 0 )

            {
                return Conflict("Phone Number already exist");
            }
            var hasher = new PasswordHasher<UserAccount>();

            var userDetails = new UserAccount(createUserRequest.Name.ToLower(), createUserRequest.Email.ToLower(), createUserRequest.PhoneNumber.ToLower(), createUserRequest.ProfileId, createUserRequest.IsSuspended);
            {
                userDetails.setPassword(hasher.HashPassword(userDetails, createUserRequest.HashedPassword));

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
