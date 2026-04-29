using CSIT_314_Group.Data;
using CSIT_314_Group.DTO.UserAccountDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.UserAccountControllers
{ 

    [ApiController]
    [Route("api/[controller]")]
    public class UpdateUserAccountController : ControllerBase
    {
        private readonly UserAccount _userAccountRepository;
        private readonly UserProfile _userProfileRepository;
        private readonly PasswordHasher<UserAccount> _passwordHasher;
        public UpdateUserAccountController(UserAccount userAccountRepository, UserProfile userProfileRepository, PasswordHasher<UserAccount> passwordHasher)
        {
            _userAccountRepository = userAccountRepository;
            _userProfileRepository = userProfileRepository;
            _passwordHasher = passwordHasher;
        }
        
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateUserAccount([FromBody] UpdateUserDTO updateUserDTO)
        {
            List<string> itemsUpdated = new List<string>();
            int userId = updateUserDTO.Id;

            UserAccount? user = await _userAccountRepository.GetAllDetailsById(userId);

            if (user == null)
            {
                return NotFound($"User with Id {userId} not found");
            }
            int? profileId = await _userProfileRepository.getIdWithProfileName(updateUserDTO.ProfileName.ToLower());

            if (!string.IsNullOrWhiteSpace(updateUserDTO.ProfileName) && profileId == null)
            {
                return BadRequest($"Invalid Profile {updateUserDTO.ProfileName.ToLower()}");
            }

            if (!string.IsNullOrWhiteSpace(updateUserDTO.Password))
            {
                var verifyPassword = _passwordHasher.VerifyHashedPassword(user, user.HashedPassword, updateUserDTO.Password);
                if (verifyPassword == PasswordVerificationResult.Success)
                {
                    return Conflict("New password is same as old password");
                }
            }

            if (!string.IsNullOrWhiteSpace(updateUserDTO.ProfileName))
            {


                var result = await _userAccountRepository.UpdateProfileById(userId, profileId);
                if (!result)
                {
                    return StatusCode(500, "Failed to update profile");
                }

                itemsUpdated.Add("ProfileName");
            }

            if (!string.IsNullOrWhiteSpace(updateUserDTO.Email))
            {
                var result = await _userAccountRepository.UpdateEmailById(userId, updateUserDTO.Email.ToLower());
                if (!result)
                {
                    return StatusCode(500, "Failed to update email");
                }

                itemsUpdated.Add("Email");
            }

            if (!string.IsNullOrWhiteSpace(updateUserDTO.PhoneNumber))
            {
                var result = await _userAccountRepository.UpdatePhoneNumberById(userId, updateUserDTO.PhoneNumber);
                if (!result)
                {
                    return StatusCode(500, "Failed to update phone number");
                }

                itemsUpdated.Add("PhoneNumber");
            }

            if (!string.IsNullOrWhiteSpace(updateUserDTO.Name))
            {
                var result = await _userAccountRepository.UpdateNameById(userId, updateUserDTO.Name.ToLower());
                if (!result)
                {
                    return StatusCode(500, "Failed to update name");
                }

                itemsUpdated.Add("Name");
            }

            if (!string.IsNullOrWhiteSpace(updateUserDTO.Password))
            {


                string hashedPassword = _passwordHasher.HashPassword(user, updateUserDTO.Password);
                var result = await _userAccountRepository.UpdatePasswordById(userId, hashedPassword);
                if (!result)
                {
                    return StatusCode(500, "Failed to update password");
                }

                itemsUpdated.Add("Password");
            }

            if (itemsUpdated.Count == 0)
            {
                return BadRequest("No fields provided to update");
            }

            return Ok(new
            {
                message = "User updated successfully",
                updatedFields = itemsUpdated
            });
        }
    }
}
