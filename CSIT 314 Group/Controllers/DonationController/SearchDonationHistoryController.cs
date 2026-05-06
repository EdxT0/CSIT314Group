using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CSIT_314_Group.Controllers.DonationController
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchDonationHistoryController : ControllerBase
    {

        private readonly FundraiserDonations _fundraiserDonationsRepository;


        public SearchDonationHistoryController(FundraiserDonations fundraiserDonationsRepository)
        {
            _fundraiserDonationsRepository = fundraiserDonationsRepository;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SearchDonation([FromQuery] string fraName)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier);
            if (user != null)
            {
                int userId = Convert.ToInt32(user.Value);
                return Ok(await _fundraiserDonationsRepository.SearchDonations(fraName, userId));
            }
            return BadRequest("no logged in user found");
        }
    }
}
