using CSIT_314_Group.Data;
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
        public async Task<IActionResult> UpdateUserAccount([FromBody] UserAccount updateUserDTO)
        {
            List<string> itemsUpdated = new List<string>();
            int userId = updateUserDTO.Id;

            UserAccount? user = await _userAccountRepository.GetAllDetailsById(userId);

            if (user == null)
            {
                return NotFound($"User with Id {userId} not found");
            }
            if (!string.IsNullOrWhiteSpace(updateUserDTO.Name))
            {
                user.Name = updateUserDTO.Name;
                itemsUpdated.Add(updateUserDTO.Name);
            }
            if (!string.IsNullOrWhiteSpace(updateUserDTO.Email))
            {
                user.Email = updateUserDTO.Email;
                itemsUpdated.Add(updateUserDTO.Email);

            }
            if (!string.IsNullOrWhiteSpace(updateUserDTO.PhoneNumber))
            {
                user.PhoneNumber = updateUserDTO.PhoneNumber;
                itemsUpdated.Add(updateUserDTO.PhoneNumber);
            }
            if (!string.IsNullOrWhiteSpace(updateUserDTO.ProfileName))
            {
                int profileId = await _userProfileRepository.getIdWithProfileName(updateUserDTO.ProfileName.ToLower()) ?? -1;
                if (profileId == -1)
                {
                    return BadRequest($"no profile with {updateUserDTO.ProfileName} found");
                }
                user.ProfileId = profileId;
                itemsUpdated.Add(updateUserDTO.ProfileName);
            }
            if (!string.IsNullOrWhiteSpace(updateUserDTO.HashedPassword))
            {
                user.HashedPassword = _passwordHasher.HashPassword(user, updateUserDTO.HashedPassword);
                itemsUpdated.Add(updateUserDTO.HashedPassword);
            }

            string result = await _userAccountRepository.UpdateDetailsById(user);

            return Ok($"{result} items updated: {(itemsUpdated.Any() ? string.Join(" , ", itemsUpdated) : 0) }");

        }
    }
}
