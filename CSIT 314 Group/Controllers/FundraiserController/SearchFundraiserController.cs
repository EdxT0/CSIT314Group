using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.FundraiserController
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchFundraiserController : ControllerBase
    {

        private readonly FundraiserActivityRepository _fundraiserActivityRepository;
        public SearchFundraiserController(FundraiserActivityRepository fundraiserActivityRepository, UserFundraiserRepository userFundraiserRepo)
        {
            _fundraiserActivityRepository = fundraiserActivityRepository;
        }
        [Authorize(Roles = "fundraiser manager, admin")]
        [HttpGet]
        public async Task<IActionResult> SearchFundraiser([FromQuery] string name)
        {
            var fundraiser = await _fundraiserActivityRepository.GetByName(name.ToLower());
            if(fundraiser == null)
            {
                return NotFound($"No fundraiser with {name} found");
            }
            return Ok(fundraiser);
        }
    }
}
