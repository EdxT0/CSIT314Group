using CSIT_314_Group.Data;
using CSIT_314_Group.DTO.UserProfileDTO;
using CSIT_314_Group.Entity;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.ProfileController;

[ApiController]
[Route("[controller]")]

public class CreateUserProfileController : ControllerBase
{
    private readonly UserProfileRepository _userProfileRepository;

    public CreateUserProfileController(UserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProfile([FromBody] CreateUserProfileDTO request)
    {
        var userProfile = new UserProfile
        {
            Id = request.Id,
            ProfileName = request.ProfileName?.Trim().ToLower(), // added trim to remove space
            Description = request.Description?.Trim().ToLower(),
            Status = "Active"
        };
        
        var result = await _userProfileRepository.CreateUserProfile(userProfile);

        if (result)
            return Ok("Profile created successfully");
        return BadRequest("Profile creation failed");
    }
}