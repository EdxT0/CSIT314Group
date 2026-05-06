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

        private readonly Data.FundraiserActivity _fundraiserActivityRepository;
        private readonly UserFundraiser _userFundraiserRepository;
        private readonly Data.Category _categoryRepository;
        public UpdateFundraiserController(Data.FundraiserActivity fundraiserActivityRepository, UserFundraiser userFundraiserRepo, Data.Category categoryRepository)
        {
            _fundraiserActivityRepository = fundraiserActivityRepository;
            _userFundraiserRepository = userFundraiserRepo;
            _categoryRepository = categoryRepository;
        }

        [Authorize(Roles = "fundraiser manager, admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateFundraiser([FromBody] FundraiserActivity updateFundraiserDTO)
        {
            List<string> itemsUpdated = new List<string>();
            var fundraiser = await _fundraiserActivityRepository.GetById(updateFundraiserDTO.Id);
            var userExist = User.FindFirst(ClaimTypes.NameIdentifier);
            if ( userExist == null)
            {
                return BadRequest("please log in as fundraiser manager or admin");
            }
            int userId = Convert.ToInt32(userExist.Value);

            bool doesFraBelongToUser = await _userFundraiserRepository.validateUserAndFundraiser(userId, updateFundraiserDTO.Id);

            if (doesFraBelongToUser || User.FindFirst(ClaimTypes.Role).Value == "admin")
            {

                if (fundraiser == null)
                {
                    return BadRequest($"Failed to find Fundraiser: {updateFundraiserDTO.Id}");
                }

                if (!string.IsNullOrWhiteSpace(updateFundraiserDTO.Name))
                {
                    if (fundraiser.Name == updateFundraiserDTO.Name)
                    {
                        return Conflict($"{updateFundraiserDTO.Name} is the same as the previous name");
                    }
                }
                if (!string.IsNullOrWhiteSpace(updateFundraiserDTO.DeadlineInString))
                {
                    if (!DateTime.TryParseExact(updateFundraiserDTO.DeadlineInString,
                                            "dd-MM-yyyy",
                                            null,
                                            System.Globalization.DateTimeStyles.None,
                                            out DateTime parsedDeadline))
                    {
                        return BadRequest($"{updateFundraiserDTO.DeadlineInString} must be in dd-MM-yyyy format");
                    }
                    if (fundraiser.DeadlineInString == updateFundraiserDTO.DeadlineInString)
                    {
                        return Conflict($"{updateFundraiserDTO.DeadlineInString} is the same as the previous deadline");
                    }
                }

                if (updateFundraiserDTO.AmtRequested != null)
                {
                    if (fundraiser.AmtRequested == updateFundraiserDTO.AmtRequested)
                    {
                        return Conflict($"{updateFundraiserDTO.AmtRequested} is the same as the previous amount requested");
                    }
                }


                if (!string.IsNullOrWhiteSpace(updateFundraiserDTO.Name))
                {
                    bool updateSuccess = await _fundraiserActivityRepository.updateName(updateFundraiserDTO.Name, fundraiser.Id);
                    if (!updateSuccess)
                    {
                        return StatusCode(500, "Failed to update Fundraiser Name");
                    }
                    itemsUpdated.Add(updateFundraiserDTO.Name);
                }
                if (!string.IsNullOrWhiteSpace(updateFundraiserDTO.Description))
                {
                    bool updateSuccess = await _fundraiserActivityRepository.UpdateDesc(updateFundraiserDTO.Description, fundraiser.Id);
                    if (!updateSuccess)
                    {
                        return StatusCode(500, "Failed to update Fundraiser Description");
                    }
                    itemsUpdated.Add(updateFundraiserDTO.Description);

                }
                if (!string.IsNullOrWhiteSpace(updateFundraiserDTO.DeadlineInString))
                {
                    bool convertToDateTimesuccess = DateTime.TryParseExact(updateFundraiserDTO.DeadlineInString,
                                           "dd-MM-yyyy",
                                           CultureInfo.InvariantCulture,
                                           DateTimeStyles.None,
                                           out DateTime parsedDeadLine);
                    if (!convertToDateTimesuccess)
                    {
                        return BadRequest("failed to convert to datetime");
                    }
                    string databaseDeadlineFormatParsedString = parsedDeadLine.ToString("O");



                    bool updateSuccess = await _fundraiserActivityRepository.UpdateDeadline(databaseDeadlineFormatParsedString, fundraiser.Id);
                    if (!updateSuccess)
                    {
                        return StatusCode(500, "Failed to update Fundraiser Deadline");
                    }
                    itemsUpdated.Add(updateFundraiserDTO.DeadlineInString);

                }
                if (updateFundraiserDTO.Status != null)
                {
                    bool updateSuccess = await _fundraiserActivityRepository.UpdateStatus(updateFundraiserDTO.Status, fundraiser.Id);
                    if (!updateSuccess)
                    {
                        return StatusCode(500, "Failed to update Fundraiser Status");
                    }
                    itemsUpdated.Add(updateFundraiserDTO.Status.ToString());

                }

                if (updateFundraiserDTO.AmtRequested != null)
                {
                    bool updateSuccess = await _fundraiserActivityRepository.UpdateAmtRequested(updateFundraiserDTO.AmtRequested, fundraiser.Id);

                    if (!updateSuccess)
                    {
                        return StatusCode(500, "Failed to update Fundraiser amount requested");
                    }
                    itemsUpdated.Add(updateFundraiserDTO.AmtRequested.ToString());
                }
                if (updateFundraiserDTO.FraCategoryId != null)
                {
                    if(await _categoryRepository.GetById(updateFundraiserDTO.FraCategoryId) == null)
                    {
                        return BadRequest("no such fundraiser category");
                    }
                    bool updateSuccess = await _fundraiserActivityRepository.UpdateFraCate(updateFundraiserDTO.FraCategoryId, fundraiser.Id);

                    if (!updateSuccess)
                    {
                        return StatusCode(500, "Failed to update Fundraiser amount requested");
                    }
                    itemsUpdated.Add(updateFundraiserDTO.FraCategoryId.ToString());
                }

                return Ok(itemsUpdated);
            }
            return BadRequest($"Fundraiser Activity {fundraiser.Name} doesnt belong to {User.FindFirstValue(ClaimTypes.Name)}");
        }
    }
}
