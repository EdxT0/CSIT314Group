using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CSIT_314_Group.Controllers.ProfileController;

[ApiController]
[Route("api/[controller]")]
public class SearchUserProfileController(UserProfileRepository userProfileRepository) : ControllerBase
{
    [Authorize(Roles ="admin")]
    [HttpGet]
    public async Task<IActionResult> SearchUserProfile([FromQuery] string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return BadRequest("Keyword cannot be empty");

        var profiles = await userProfileRepository.SearchUserProfile(keyword);

        if (profiles == null || profiles.Count == 0)
            return NotFound("No matching user profiles found");

        return Ok(profiles);
    }
}