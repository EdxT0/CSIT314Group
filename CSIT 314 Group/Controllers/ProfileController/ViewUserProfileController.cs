using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.ProfileController;

[ApiController]
[Route("[controller]")]
public class ViewUserProfileController(UserProfileRepository userProfileRepository) : ControllerBase
{
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