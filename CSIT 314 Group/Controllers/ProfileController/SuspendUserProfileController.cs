using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.ProfileController;

[ApiController]
[Route("api/[controller]")]
public class SuspendUserProfileController(UserProfileRepository userProfileRepository) : ControllerBase
{
    [HttpPut]
    public async Task<IActionResult> SuspendUserProfile(int id, string status)
    {
        status = status.Trim();

        if (status != "Suspended" && status != "Active")
            return BadRequest("Status must be either 'Suspended' or 'Active'");

        var result = await userProfileRepository.SuspendUserProfile(id, status);

        if (!result)
            return NotFound("User profile not found");
        return Ok($"'User profile status updated to {status}");
    }
    
}

