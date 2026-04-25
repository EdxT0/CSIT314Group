using CSIT_314_Group.Data;
using CSIT_314_Group.DTO.UserProfileDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.ProfileController;

[ApiController]
[Route("api/[controller]")]
public class SuspendUserProfileController(UserProfileRepository userProfileRepository) : ControllerBase
{
    [Authorize(Roles ="admin")]
    [HttpPut]
    public async Task<IActionResult> SuspendUserProfile([FromBody]SuspendProfileDTO? SuspendProfileDTO)
    {
        if (SuspendProfileDTO == null)
            return BadRequest("Request body cannot be empty");
        
        if (SuspendProfileDTO.id <= 0)
            return BadRequest("Invalid Profile ID");
        
        var result = await userProfileRepository.SuspendUserProfile(
            SuspendProfileDTO.id, 
            SuspendProfileDTO.isSuspend);

        if (!result)
            return NotFound("User profile not found");
        
        return Ok($"'User profile status updated to {SuspendProfileDTO.isSuspend}");
    }
    
}

