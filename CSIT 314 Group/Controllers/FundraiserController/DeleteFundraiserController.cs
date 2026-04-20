using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;
using System.Security.Claims;

namespace CSIT_314_Group.Controllers.FundraiserActivity
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteFundraiserController : ControllerBase
    {

        private readonly FundraiserActivityRepository _fundraiserActivityRepository;
        private readonly UserFundraiserRepository _userFundraiserRepository;
        public DeleteFundraiserController(FundraiserActivityRepository fundraiserActivityRepository, UserFundraiserRepository userFundraiserRepo)
        {
            _fundraiserActivityRepository = fundraiserActivityRepository;
            _userFundraiserRepository = userFundraiserRepo;
        }
        [Authorize(Roles = "fundraiser manager, admin")]
        [HttpDelete]
        public async Task<IActionResult> DeleteFundraiser([FromQuery] int fundraiserId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var fundraiser = await _fundraiserActivityRepository.GetById(fundraiserId);

            if (userId == null)
            {
                return BadRequest("please log in as fundraiser manager or admin");
            }
            if (fundraiser == null)
            {
                return BadRequest($"unable to find fundraiser with id: {fundraiserId}");
            }
            bool fundraiserBelongToUser = await _userFundraiserRepository.validateUserAndFundraiser(Convert.ToInt32(userId), fundraiserId);
            if (fundraiserBelongToUser || User.FindFirstValue(ClaimTypes.Role) == "admin")
            {

                bool success = await _fundraiserActivityRepository.DeleteFundraiser(fundraiserId);
                if (success)
                {
                    return Ok($"Fundraiser {fundraiser.Name} Successfully deleted");
                }
                return StatusCode(500, "Failed to delete Fundraiser");
            }
            return BadRequest($"Fundraiser {fundraiser.Name} doesnt below to {User.FindFirstValue(ClaimTypes.Name)}");
        }
    }
}
