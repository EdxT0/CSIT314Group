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
        public async Task<IActionResult> UpdateUserAccount([FromBody] UserAccount updateUser)
        {
            List<string> itemsUpdated = new List<string>();
            int userId = updateUser.Id;

            UserAccount? user = await _userAccountRepository.GetAllDetailsById(userId);

            if (user == null)
            {
                return NotFound($"User with Id {userId} not found");
            }
            if (!string.IsNullOrWhiteSpace(updateUser.Name))
            {
                user.Name = updateUser.Name;
                itemsUpdated.Add(updateUser.Name);
            }
            if (!string.IsNullOrWhiteSpace(updateUser.Email))
            {
                user.Email = updateUser.Email;
                itemsUpdated.Add(updateUser.Email);

            }
            if (!string.IsNullOrWhiteSpace(updateUser.PhoneNumber))
            {
                user.PhoneNumber = updateUser.PhoneNumber;
                itemsUpdated.Add(updateUser.PhoneNumber);
            }
            if (!string.IsNullOrWhiteSpace(updateUser.ProfileName))
            {
                int profileId = await _userProfileRepository.getIdWithProfileName(updateUser.ProfileName.ToLower()) ?? -1;
                if (profileId == -1)
                {
                    return BadRequest($"no profile with {updateUser.ProfileName} found");
                }
                user.ProfileId = profileId;
                itemsUpdated.Add(updateUser.ProfileName);
            }
            if (!string.IsNullOrWhiteSpace(updateUser.HashedPassword))
            {
                user.HashedPassword = _passwordHasher.HashPassword(user, updateUser.HashedPassword);
                itemsUpdated.Add(updateUser.HashedPassword);
            }

            string result = await _userAccountRepository.UpdateDetailsById(user);

            return Ok($"{result} items updated: {(itemsUpdated.Any() ? string.Join(" , ", itemsUpdated) : 0) }");

        }
    }
}
