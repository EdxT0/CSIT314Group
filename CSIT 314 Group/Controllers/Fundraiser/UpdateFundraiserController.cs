using CSIT_314_Group.Data;
using CSIT_314_Group.DTO.FundraiserActivityDTO;
using CSIT_314_Group.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.FundraiserActivity
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateFundraiserController : ControllerBase
    {
        private readonly FundraiserActivityRepository _fundraiserActivityRepository;

        public UpdateFundraiserController(FundraiserActivityRepository fundraiserActivityRepository)
        {
            _fundraiserActivityRepository = fundraiserActivityRepository;
        }

        [Authorize(Roles = "platform manager")]
        [HttpPut]
        public async Task<IActionResult> UpdateFundraiser([FromBody] UpdateFundraiserDTO updateFundraiserDTO)
        {
            if (!DateTime.TryParseExact(updateFundraiserDTO.Deadline,
                    "dd-MM-yyyy",
                    null,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime parsedDeadline))
            {
                return BadRequest("Deadline must be in dd-MM-yyyy format");
            }

            var existingFundraiser = await _fundraiserActivityRepository.GetById(updateFundraiserDTO.Id);

            if (existingFundraiser == null)
            {
                return NotFound("Fundraiser not found");
            }

            var fundraiser = new Fundraiser(
                updateFundraiserDTO.Name.ToLower(),
                updateFundraiserDTO.Description,
                parsedDeadline,
                updateFundraiserDTO.AmtRequested
            );

            fundraiser.Id = updateFundraiserDTO.Id;
            fundraiser.Status = updateFundraiserDTO.Status;

            bool result = await _fundraiserActivityRepository.UpdateFundraiser(fundraiser);

            if (!result)
            {
                return StatusCode(500, "Failed to update fundraiser");
            }

            return Ok($"Updated fundraiser with Id {updateFundraiserDTO.Id}");

        }
    }
}
