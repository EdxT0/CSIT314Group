using CSIT_314_Group.Data;
using CSIT_314_Group.DTO.UserProfileDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CSIT_314_Group.Controllers.ProfileController;

[ApiController]
[Route("api/[controller]")]

public class CreateUserProfileController : ControllerBase
{
    private readonly Data.UserProfile _userProfileRepository;

    public CreateUserProfileController(Data.UserProfile userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }
    
    [Authorize(Roles ="admin")]
    [HttpPost]
    public async Task<IActionResult> CreateProfile([FromBody] CreateUserProfileDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.ProfileName) ||
            string.IsNullOrWhiteSpace(request.Description))
        {
            return BadRequest("Profile name and description cannot be empty");
        }

        var profileName = request.ProfileName.Trim().ToLower();
        var exists = await _userProfileRepository.ProfileNameExists(profileName);
        if (exists)
        {
            return BadRequest("Profile already exists");
        }

        var userProfile = new UserProfile
        {
            ProfileName = request.ProfileName.Trim().ToLower(),
            Description = request.Description.ToLower(),
            Status = false
        };

        var result = await _userProfileRepository.CreateUserProfile(userProfile);

        if (result)
            return Ok("Profile created successfully");

        return BadRequest("Profile creation failed");
    }
}