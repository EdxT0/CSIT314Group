using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections;


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

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserAccount createUserRequest)
        {

            if (createUserRequest.ProfileId == null)
            {
                return BadRequest("Invalid Profile");
            }
            if ((await _userAccountRepository.GetIdsWithQuery(createUserRequest.Email)).Count != 0)
            {
                return BadRequest("email already exist");
            }

            if ((await _userAccountRepository.GetIdsWithQuery(createUserRequest.PhoneNumber)).Count != 0)

            {
                return BadRequest("Phone Number already exist");
            }
            var hasher = new PasswordHasher<UserAccount>();

            var userDetails = new UserAccount(createUserRequest.Name.ToLower(), createUserRequest.Email.ToLower(), createUserRequest.PhoneNumber.ToLower(), createUserRequest.ProfileId, createUserRequest.IsSuspended);
            {
                userDetails.setPassword(hasher.HashPassword(userDetails, createUserRequest.HashedPassword));

                var result = await _userAccountRepository.CreateUser(userDetails);
                if (result.success)
                {
                    return Ok(result.message);
                }
                else
                {
                    return BadRequest(result.message);
                }
            }
            ;
        }

    }
}

