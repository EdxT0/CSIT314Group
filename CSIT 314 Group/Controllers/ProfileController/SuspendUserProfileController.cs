using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.ProfileController;

[ApiController]
[Route("api/[controller]")]
public class SuspendUserProfileController(UserProfile userProfileRepository) : ControllerBase
{
    [Authorize(Roles ="admin")]
    [HttpPut]
    public async Task<IActionResult> SuspendUserProfile([FromBody] UserProfile SuspendProfileDTO)
    {
        if (SuspendProfileDTO == null)
            return BadRequest("Request body cannot be empty");
        
        if (SuspendProfileDTO.Id <= 0)
            return BadRequest("Invalid Profile ID");
        
        var result = await userProfileRepository.SuspendUserProfile(
            SuspendProfileDTO.Id, 
            SuspendProfileDTO.Status);

        if (!result)
            return NotFound("User profile not found");
        
        return Ok($"'User profile status updated to {SuspendProfileDTO.Status}");
    }
    
}

