using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CSIT_314_Group.Controllers.DonationController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddDonationController : ControllerBase
    {
        private readonly FundraiserDonations _fundraiserDonationsRepository;
        private readonly FundraiserActivity _fundraiserActivity;

        public AddDonationController(FundraiserDonations fundraiserDonationsRepository, Data.FundraiserActivity fundraiserActivity)
        {
            _fundraiserDonationsRepository = fundraiserDonationsRepository;
            _fundraiserActivity = fundraiserActivity;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddDonation( [FromBody] FundraiserDonations fundraiserDonations)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier);
            if (user == null)
            {
                return BadRequest("No log in detected");
            }
            int userId = Convert.ToInt32(user.Value);
            var FundraiserAmtRequested = await _fundraiserActivity.getAmtRequested(fundraiserDonations.Id);
            if (FundraiserAmtRequested == null)
            {
                return BadRequest("Fundraiser not found");
            }
            double FundraiserAmtDonated = Convert.ToDouble(await _fundraiserActivity.getAmtDonated(fundraiserDonations.Id));
            double amtRequested = Convert.ToDouble(FundraiserAmtRequested);
            if (!isDonationValid(FundraiserAmtDonated, amtRequested, fundraiserDonations.AmtDonated))
            {
                return BadRequest("user Donated amount is bigger than amount requested after adding current donated amount");
            }
            DateTime dateTimeNow = DateTime.Now;
            double newAmountDonated = FundraiserAmtDonated + fundraiserDonations.AmtDonated;

            bool updateToFundraiserDonationSuccess = await _fundraiserDonationsRepository.AddDonation(userId, fundraiserDonations.Id, fundraiserDonations.AmtDonated, dateTimeNow);
            if (updateToFundraiserDonationSuccess)
            {
                bool updateToFundraiserActivitySuccess = await _fundraiserActivity.UpdateAmtDonated(newAmountDonated, fundraiserDonations.Id);
                if (updateToFundraiserActivitySuccess)
                {
                    return Ok("amount donated successfully");

                }
                else
                {
                    bool deleteDonationSuccess = await _fundraiserDonationsRepository.DeleteDonation(userId, fundraiserDonations.Id, fundraiserDonations.AmtDonated, dateTimeNow);
                    if (deleteDonationSuccess)
                    {
                        return StatusCode(500, "Failed to add donations, but database successfully cleaned");
                    }
                    return StatusCode(500, "failed to add donations, FundraiserDonation failed to clean");
                }
            }
            return StatusCode(500, "failed to update server");

        }

        private bool isDonationValid(double fundraiserAmtDonated, double amtRequested, double userDonated)
        {
            return userDonated <= amtRequested && fundraiserAmtDonated + userDonated <= amtRequested;
        }
    }
}
