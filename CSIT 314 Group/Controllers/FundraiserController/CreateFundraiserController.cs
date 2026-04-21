using CSIT_314_Group.Data;
using CSIT_314_Group.Entity;
using CSIT_314_Group.DTO.FundraiserActivityDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CSIT_314_Group.Controllers.FundraiserActivity
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreateFundraiserController : ControllerBase
    {
        private readonly FundraiserActivityRepository _fundraiserActivityRepository;
        private readonly UserFundraiserRepository _userFundraiserRepo;
        public CreateFundraiserController(FundraiserActivityRepository fundraiserActivityRepository, UserFundraiserRepository userFundraiserRepo)
        {
            _fundraiserActivityRepository = fundraiserActivityRepository;
            _userFundraiserRepo = userFundraiserRepo;
        }

        [Authorize(Roles = "fundraiser manager")]
        [HttpPost]
        public async Task<IActionResult> CreateFundraiser([FromBody] CreateFundraiserDTO createFundraiserDTO)
        {

            if (!DateTime.TryParseExact(createFundraiserDTO.Deadline,
                                        "dd-MM-yyyy",
                                        null,
                                        System.Globalization.DateTimeStyles.None,
                                        out DateTime parsedDeadline))
            {
                return BadRequest("Deadline must be in dd-MM-yyyy format");
            }
            var result = await _fundraiserActivityRepository.GetByName(createFundraiserDTO.Name.ToLower());

            if (result == null)
            {
                var fundraiser = new Fundraiser(createFundraiserDTO.Name.ToLower(),
                                                createFundraiserDTO.Description,
                                                parsedDeadline,
                                                createFundraiserDTO.amtRequested);

                int? fraId = await _fundraiserActivityRepository.createFundraiser(fundraiser);
                if (fraId != null)
                {
                    int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    if (await _userFundraiserRepo.AddFRAToUser(userId, fraId))
                    {
                        return Ok($"Created {fundraiser.Name} Fundraiser");
                    }
                }
                return StatusCode(500, "Failed to create fundraiser");
            }
            return Conflict($"Fundraiser with the same name ( {createFundraiserDTO.Name} )exists already");
        }
    }
}
