using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

using System.Security.Claims;

namespace CSIT_314_Group.Controllers.FundraiserController
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteFundraiserController : ControllerBase
    {

        private readonly Data.FundraiserActivity _fundraiserActivityRepository;
        private readonly UserFundraiser _userFundraiserRepository;
        public DeleteFundraiserController(FundraiserActivity fundraiserActivityRepository, UserFundraiser userFundraiserRepo)
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
                try
                {
                    var success = await _fundraiserActivityRepository.DeleteFundraiser(fundraiserId);
                    if (success.Equals("deleted"))
                    {
                        return Ok($"Fundraiser {fundraiser.Name} Successfully deleted");
                    }
                
                }catch(SqliteException ex) when (ex.SqliteErrorCode == 19){
                    return BadRequest("Fundraiser has donations made to it already");
                }
                return StatusCode(500, "Failed to delete Fundraiser");
            }
            return BadRequest($"Fundraiser {fundraiser.Name} doesnt below to {User.FindFirstValue(ClaimTypes.Name)}");
        }
    }
}
