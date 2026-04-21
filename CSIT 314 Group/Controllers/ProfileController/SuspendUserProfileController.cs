using CSIT_314_Group.Data;
using CSIT_314_Group.DTO.UserProfileDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.ProfileController;

[ApiController]
[Route("api/[controller]")]
public class SuspendUserProfileController(UserProfileRepository userProfileRepository) : ControllerBase
{
    [Authorize]
    [HttpPut]
    public async Task<IActionResult> SuspendUserProfile([FromBody]SuspendProfileDTO suspendProfileDTO)
    {
        

        var result = await userProfileRepository.SuspendUserProfile(suspendProfileDTO.id, suspendProfileDTO.isSuspend);

        if (!result)
            return NotFound("User profile not found");
        return Ok($"'User profile status updated to {suspendProfileDTO.isSuspend}");
    }
    
}

