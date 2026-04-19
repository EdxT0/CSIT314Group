using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CSIT_314_Group.Controllers.FundraiserController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewMyFundraisersController : ControllerBase
    {
        private readonly UserFundraiserRepository _userFundraiserRepo;
        public ViewMyFundraisersController(UserFundraiserRepository userFundraiserRepo)
        {
            _userFundraiserRepo = userFundraiserRepo;
        }


        [Authorize(Roles = "fundraiser manager")]
        [HttpGet]
        public async Task<IActionResult> ViewMyFundraisers()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized("User ID claim not found.");
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Invalid user ID claim.");
            }
            var result = await _userFundraiserRepo.ViewMyFundraisers(userId);
            if (result.Count != 0 )
            {
                return Ok(result);
            }

            return NotFound("No Fundraisers made yet");
        }
    }
}
