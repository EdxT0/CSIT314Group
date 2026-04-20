using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.FundraiserActivity
{

    [Route("api/[controller]")]
    [ApiController]
    public class ViewAllFundraiserController : ControllerBase
    {
        private readonly FundraiserActivityRepository _fundraiserActivityRepository;
        public ViewAllFundraiserController(FundraiserActivityRepository fundraiserActivityRepository)
        {
            _fundraiserActivityRepository = fundraiserActivityRepository;
        }
        [HttpGet]
        public async Task<IActionResult> ViewAllFundraiser()
        {
            var result = await _fundraiserActivityRepository.ViewAllFundraisers();
            if (result.Count == 0)
            {
                return StatusCode(404, "No Fundraiser Activities yet");
            }
            return Ok(result);
        }
    }
}
