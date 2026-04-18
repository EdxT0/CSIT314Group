using CSIT_314_Group.Data;
using CSIT_314_Group.DTO.UserProfileDTO;
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
        var result = await _userProfileRepository.CreateUserProfile(request.ProfileName);
        
        if (result)
            return Ok("Profile created successfully");
        
        return BadRequest("Profile creation failed");
    }
}