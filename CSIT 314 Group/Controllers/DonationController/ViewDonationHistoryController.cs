using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace CSIT_314_Group.Controllers.DonationController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewDonationHistoryController : ControllerBase
    {
        private readonly FundraiserDonations _fundraiserDonationsRepository;


        public ViewDonationHistoryController(FundraiserDonations fundraiserDonationsRepository)
        {
            _fundraiserDonationsRepository = fundraiserDonationsRepository;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ViewDonation()
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier);
            if(user != null)
            {
                int userId = Convert.ToInt32(user.Value);
                return Ok( await _fundraiserDonationsRepository.ViewDonatedFundraiser(userId));
            }
            return BadRequest("no logged in user found");
        }
    }
}
