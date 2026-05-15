using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace CSIT_314_Group.Controllers.FundraiserController
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateFundraiserController : ControllerBase
    {

        private readonly FundraiserActivity _fundraiserActivityRepository;
        private readonly UserFundraiser _userFundraiserRepository;
        private readonly Category _categoryRepository;
        public UpdateFundraiserController(Data.FundraiserActivity fundraiserActivityRepository, UserFundraiser userFundraiserRepo, Data.Category categoryRepository)
        {
            _fundraiserActivityRepository = fundraiserActivityRepository;
            _userFundraiserRepository = userFundraiserRepo;
            _categoryRepository = categoryRepository;
        }



        [Authorize(Roles = "fundraiser manager,admin")]  // ← fix: no spaces after comma
        [HttpPut]
        public async Task<IActionResult> UpdateFundraiser([FromBody] FundraiserActivity updateFundraiserDTO)
        {
            var userExist = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userExist == null)
                return BadRequest("please log in as fundraiser manager or admin");

            var fundraiser = await _fundraiserActivityRepository.GetById(updateFundraiserDTO.Id);
            if (fundraiser == null)
                return BadRequest("no such Fundraiser exists");

            int userId = Convert.ToInt32(userExist.Value);

            if (!await _userFundraiserRepository.validateUserAndFundraiser(userId, updateFundraiserDTO.Id))
                return BadRequest($"Fundraiser Activity {fundraiser.Name} doesnt belong to {User.FindFirstValue(ClaimTypes.Name)}");

            List<string> itemsUpdated = new List<string>();

            if (!string.IsNullOrWhiteSpace(updateFundraiserDTO.Name))
            {
                fundraiser.Name = updateFundraiserDTO.Name;
                itemsUpdated.Add("Name");
            }
            if (!string.IsNullOrWhiteSpace(updateFundraiserDTO.Description))
            {
                fundraiser.Description = updateFundraiserDTO.Description;
                itemsUpdated.Add("Description");
            }
            if (!string.IsNullOrWhiteSpace(updateFundraiserDTO.DeadlineInString))
            {
                bool parsed = DateTime.TryParseExact(updateFundraiserDTO.DeadlineInString, "dd-MM-yyyy",
                    null, System.Globalization.DateTimeStyles.None, out DateTime parsedDeadline);
                if (!parsed)
                    return BadRequest("Deadline must be in dd-MM-yyyy format");
                fundraiser.Deadline = parsedDeadline;
                itemsUpdated.Add("Deadline");
            }
            if (updateFundraiserDTO.AmtRequested.HasValue && updateFundraiserDTO.AmtRequested != fundraiser.AmtRequested)
            {
                fundraiser.AmtRequested = updateFundraiserDTO.AmtRequested;
                itemsUpdated.Add("AmtRequested");
            }

            // ← only update FraCategoryId if it was actually provided and is valid
            if (updateFundraiserDTO.FraCategoryId > 0 && updateFundraiserDTO.FraCategoryId != fundraiser.FraCategoryId)
            {
                var categoryExists = await _categoryRepository.GetById(updateFundraiserDTO.FraCategoryId);
                if (categoryExists == null)
                    return BadRequest("no such fundraiser category");
                fundraiser.FraCategoryId = updateFundraiserDTO.FraCategoryId;
                itemsUpdated.Add("FraCategoryId");
            }

            if (itemsUpdated.Count == 0)
                return BadRequest("No fields provided to update");

            var result = await _fundraiserActivityRepository.UpdateDetailsById(fundraiser);
            return Ok($"{result} - Updated: {string.Join(", ", itemsUpdated)}");
        }
    }
}
