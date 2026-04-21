using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.ProfileController;

[ApiController]
[Route("api/[controller]")]
public class ViewAllUserProfileController(UserProfileRepository userProfileRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ViewAllUserProfiles()
    {
        var profiles = await userProfileRepository.GetAllUserProfiles();
        return Ok(profiles);
    }
}