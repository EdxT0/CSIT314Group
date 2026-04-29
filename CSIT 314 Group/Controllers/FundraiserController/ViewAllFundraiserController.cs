using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.FundraiserController
{

    [Route("api/[controller]")]
    [ApiController]
    public class ViewAllFundraiserController : ControllerBase
    {
        private readonly Data.FundraiserActivity _fundraiserActivityRepository;
        public ViewAllFundraiserController(Data.FundraiserActivity fundraiserActivityRepository)
        {
            _fundraiserActivityRepository = fundraiserActivityRepository;
        }
        [HttpGet]
        public async Task<IActionResult> ViewAllFundraiser()
        {
            var result = await _fundraiserActivityRepository.ViewAllFundraisers();
            if (result.Count == 0)
            {
                return NotFound("No Fundraiser Activities yet");
            }
            return Ok(result);
        }
    }
}
