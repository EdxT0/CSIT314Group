using CSIT_314_Group.Data;
using CSIT_314_Group.DTO.FundraiserActivityDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CSIT_314_Group.Controllers.FundraiserController
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreateFundraiserController : ControllerBase
    {
        private readonly FundraiserActivity _fundraiserActivityRepository;
        private readonly UserFundraiserRepository _userFundraiserRepository;
        private readonly Data.Category _categoryRepository;
        public CreateFundraiserController(FundraiserActivity fundraiserActivityRepository, UserFundraiserRepository userFundraiserRepo, Category categoryRepository)
        {
            _fundraiserActivityRepository = fundraiserActivityRepository;
            _userFundraiserRepository = userFundraiserRepo;
            _categoryRepository = categoryRepository;
        }

        [Authorize(Roles = "fundraiser manager, admin")]
        [HttpPost]
        public async Task<IActionResult> CreateFundraiser([FromBody] CreateFundraiserDTO createFundraiserDTO)
        {

            if (!DateTime.TryParseExact(createFundraiserDTO.deadline,
                                        "dd-MM-yyyy",
                                        null,
                                        System.Globalization.DateTimeStyles.None,
                                        out DateTime parsedDeadline))
            {
                return BadRequest("Deadline must be in dd-MM-yyyy format");
            }

            if(createFundraiserDTO.fraCategoryId == null)
            {
                return BadRequest("Fundraiser Category cannot be empty! Please select existing Categories!");
            }
            if (await _categoryRepository.GetById(createFundraiserDTO.fraCategoryId) == null)
            {
                return BadRequest("no such fundraiser category");
            }
            var result = await _fundraiserActivityRepository.GetByName(createFundraiserDTO.name.ToLower());

            if (result == null)
            {
                var fundraiser = new FundraiserActivity(createFundraiserDTO.name.ToLower(),
                                                createFundraiserDTO.description,
                                                parsedDeadline,
                                                createFundraiserDTO.fraCategoryId,
                                                createFundraiserDTO.amtRequested);

                int? fraId = await _fundraiserActivityRepository.createFundraiser(fundraiser);
                if (fraId != null)
                {
                    int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    if (await _userFundraiserRepository.AddFRAToUser(userId, fraId))
                    {
                        return Ok($"Created {fundraiser.Name} Fundraiser");
                    }
                }
                return StatusCode(500, "Failed to create fundraiser");
            }
            return Conflict($"Fundraiser with the same name ( {createFundraiserDTO.name} )exists already");
        }
    }
}
