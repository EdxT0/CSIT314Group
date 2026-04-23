using CSIT_314_Group.Data;
using CSIT_314_Group.DTO.UserDonationDTO;
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
        private readonly FundraiserDonationsRepository _fundraiserDonationsRepository;
        private readonly FundraiserActivityRepository _fundraiserActivity;

        public AddDonationController(FundraiserDonationsRepository fundraiserDonationsRepository, FundraiserActivityRepository fundraiserActivity)
        {
            _fundraiserDonationsRepository = fundraiserDonationsRepository;
            _fundraiserActivity = fundraiserActivity;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddDonation([FromBody] AddDonationDTO addDonationDTO)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier);
            if(user == null)
            {
                return BadRequest("No log in detected");
            }
            int userId = Convert.ToInt32(user.Value);
            var FundraiserAmtRequested = await _fundraiserActivity.getAmtRequested(addDonationDTO.FraId);
            if(FundraiserAmtRequested == null)
            {
                return BadRequest("Fundraiser not found");
            }
            double FundraiserAmtDonated = Convert.ToDouble(await _fundraiserActivity.getAmtDonated(addDonationDTO.FraId));
            double amtRequested = Convert.ToDouble(FundraiserAmtRequested);
            if(!isDonationValid(FundraiserAmtDonated, amtRequested, addDonationDTO.AmtDonatedByUser))
            {
                return BadRequest("user Donated amount is bigger than amount requested after adding current donated amount");
            }
            DateTime dateTimeNow = DateTime.Now;
            double newAmountDonated = FundraiserAmtDonated + addDonationDTO.AmtDonatedByUser;
            
            bool updateToFundraiserDonationSuccess = await _fundraiserDonationsRepository.AddDonation(userId, addDonationDTO.FraId, addDonationDTO.AmtDonatedByUser, dateTimeNow);
            if (updateToFundraiserDonationSuccess)
            {
                bool updateToFundraiserActivitySuccess = await _fundraiserActivity.UpdateAmtDonated(newAmountDonated, addDonationDTO.FraId);
                if (updateToFundraiserActivitySuccess)
                {
                    return Ok("amount donated successfully");

                }
                else
                {
                    bool deleteDonationSuccess = await _fundraiserDonationsRepository.DeleteDonation(userId, addDonationDTO.FraId, addDonationDTO.AmtDonatedByUser, dateTimeNow);
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
