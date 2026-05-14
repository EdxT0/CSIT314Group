using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CSIT_314_Group.Controllers.ProfileController;

[ApiController]
[Route("api/[controller]")]
public class UpdateUserProfileController(UserProfile userProfileRepository) : ControllerBase
{
    //[Authorize(Roles ="admin")]
    [HttpPut]
    public async Task<IActionResult> UpdateUserProfile([FromBody] UserProfile request)
    {
        if (request == null)
            return BadRequest("Invalid request");

        if (string.IsNullOrWhiteSpace(request.ProfileName) ||
            string.IsNullOrWhiteSpace(request.Description))
        {
            return BadRequest("Role and Description cannot be empty");
        }

        var result = await userProfileRepository.UpdateUserProfile(request);

        if (result.Contains("successfully"))
        {
            return Ok(result);
        }
    
        return NotFound(result);
    }
}