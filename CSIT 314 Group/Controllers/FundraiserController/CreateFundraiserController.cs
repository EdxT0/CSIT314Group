using CSIT_314_Group.Data;
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
        private readonly UserFundraiser _userFundraiserRepository;
        private readonly Data.Category _categoryRepository;
        public CreateFundraiserController(FundraiserActivity fundraiserActivityRepository, UserFundraiser userFundraiserRepo, Category categoryRepository)
        {
            _fundraiserActivityRepository = fundraiserActivityRepository;
            _userFundraiserRepository = userFundraiserRepo;
            _categoryRepository = categoryRepository;
        }

        [Authorize(Roles = "fundraiser manager, admin")]
        [HttpPost]
        public async Task<IActionResult> CreateFundraiser([FromBody] FundraiserActivity createFundraiserDTO)
        {

            if (!DateTime.TryParseExact(createFundraiserDTO.DeadlineInString,
                                        "dd-MM-yyyy",
                                        null,
                                        System.Globalization.DateTimeStyles.None,
                                        out DateTime parsedDeadline))
            {
                return BadRequest("Deadline must be in dd-MM-yyyy format");
            }

            if(createFundraiserDTO.FraCategoryId == null)
            {
                return BadRequest("Fundraiser Category cannot be empty! Please select existing Categories!");
            }
            if (await _categoryRepository.GetById(createFundraiserDTO.FraCategoryId) == null)
            {
                return BadRequest("no such fundraiser category");
            }
            var result = await _fundraiserActivityRepository.GetByName(createFundraiserDTO.Name.ToLower());

            if (result == null)
            {
                createFundraiserDTO.Name = createFundraiserDTO.Name.ToLower();

                int? fraId = await _fundraiserActivityRepository.createFundraiser(createFundraiserDTO);
                if (fraId != null)
                {
                    int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    if (await _userFundraiserRepository.AddFRAToUser(userId, fraId))
                    {
                        return Ok($"Created {createFundraiserDTO.Name} Fundraiser");
                    }
                }
                return StatusCode(500, "Failed to create fundraiser");
            }
            return Conflict($"Fundraiser with the same name ( {createFundraiserDTO.Name} )exists already");
        }
    }
}
