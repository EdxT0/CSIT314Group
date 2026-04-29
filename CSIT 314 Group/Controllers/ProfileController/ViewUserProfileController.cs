using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CSIT_314_Group.Controllers.ProfileController;

[ApiController]
[Route("api/[controller]")]
public class ViewUserProfileController(UserProfile userProfileRepository) : ControllerBase
{
    [Authorize(Roles ="admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> ViewUserProfile(int id)
    {
        var profile = await userProfileRepository.GetUserProfile(id);

        if (profile == null)
        {
            return NotFound("User profile not found");
        }

        return Ok(profile);
    }
}