using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CSIT_314_Group.Controllers.ProfileController;

[ApiController]
[Route("api/[controller]")]

public class ViewAllUserProfileController : ControllerBase
{
    private readonly UserProfile _userProfileRepository;

    public ViewAllUserProfileController(UserProfile userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }

    [Authorize(Roles = "admin")]
    [HttpGet]
    public async Task<IActionResult> ViewAllUserProfiles()
    {
        var profiles = await _userProfileRepository.ViewAllUserProfiles();
        if (profiles.Count == 0)
            return NotFound("User Profile not found");

        return Ok(profiles);
    }
}