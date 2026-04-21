using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CSIT_314_Group.Controllers.FundraiserController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewOneFundraiserController : ControllerBase
    {
        private readonly FundraiserActivityRepository _fundraiserActivityRepository;
        private readonly UserFundraiserRepository _userFundraiserRepo;

        public ViewOneFundraiserController(FundraiserActivityRepository fundraiserActivityRepository, UserFundraiserRepository userFundraiserRepo)
        {
            _fundraiserActivityRepository = fundraiserActivityRepository;
            _userFundraiserRepo = userFundraiserRepo;

        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ViewOneFundraiser([FromQuery] int fraId)
        {
            var fundraiser = await _fundraiserActivityRepository.GetById(fraId);
            if(fundraiser == null)
            {
                return NotFound($"No Fundraiser with Id: {fraId} found");
            }
            var user = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if(user != null)
            {
                int userId = Convert.ToInt32(user);
                bool doesFraBelongsToUser = await _userFundraiserRepo.validateUserAndFundraiser(userId, fraId);
                if (!doesFraBelongsToUser)
                {
                    bool success = await _fundraiserActivityRepository.UpdateFundraiserView(fraId);
                    if (success)
                    {
                        var fundraiserAfterUpdate = await _fundraiserActivityRepository.GetById(fraId);

                        return Ok(fundraiserAfterUpdate);
                    }
                    return StatusCode(500, "failed to increase view");
                }

                return Ok(fundraiser);

            }
            return NotFound("User not found");
        }
    }
}
