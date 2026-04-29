using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CSIT_314_Group.DTO.FundraiserActivityDTO;
using System.Globalization;
using System.Security.Claims;

namespace CSIT_314_Group.Controllers.FundraiserController
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateFundraiserController : ControllerBase
    {

        private readonly Data.FundraiserActivity _fundraiserActivityRepository;
        private readonly UserFundraiserRepository _userFundraiserRepository;
        private readonly Data.Category _categoryRepository;
        public UpdateFundraiserController(Data.FundraiserActivity fundraiserActivityRepository, UserFundraiserRepository userFundraiserRepo, Data.Category categoryRepository)
        {
            _fundraiserActivityRepository = fundraiserActivityRepository;
            _userFundraiserRepository = userFundraiserRepo;
            _categoryRepository = categoryRepository;
        }

        [Authorize(Roles = "fundraiser manager, admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateFundraiser([FromBody] UpdateFundraiserDTO updateFundraiserDTO)
        {
            List<string> itemsUpdated = new List<string>();
            var fundraiser = await _fundraiserActivityRepository.GetById(updateFundraiserDTO.fraId);
            var userExist = User.FindFirst(ClaimTypes.NameIdentifier);
            if ( userExist == null)
            {
                return BadRequest("please log in as fundraiser manager or admin");
            }
            int userId = Convert.ToInt32(userExist.Value);

            bool doesFraBelongToUser = await _userFundraiserRepository.validateUserAndFundraiser(userId, updateFundraiserDTO.fraId);

            if (doesFraBelongToUser || User.FindFirst(ClaimTypes.Role).Value == "admin")
            {

                if (fundraiser == null)
                {
                    return BadRequest($"Failed to find Fundraiser: {updateFundraiserDTO.fraId}");
                }

                if (!string.IsNullOrWhiteSpace(updateFundraiserDTO.fraName))
                {
                    if (fundraiser.Name == updateFundraiserDTO.fraName)
                    {
                        return Conflict($"{updateFundraiserDTO.fraName} is the same as the previous name");
                    }
                }
                if (!string.IsNullOrWhiteSpace(updateFundraiserDTO.deadline))
                {
                    if (!DateTime.TryParseExact(updateFundraiserDTO.deadline,
                                            "dd-MM-yyyy",
                                            null,
                                            System.Globalization.DateTimeStyles.None,
                                            out DateTime parsedDeadline))
                    {
                        return BadRequest($"{updateFundraiserDTO.deadline} must be in dd-MM-yyyy format");
                    }
                    if (fundraiser.Deadline == updateFundraiserDTO.deadline)
                    {
                        return Conflict($"{updateFundraiserDTO.deadline} is the same as the previous deadline");
                    }
                }
                if (updateFundraiserDTO.status != null)
                {
                    if (fundraiser.Status == updateFundraiserDTO.status)
                    {
                        return Conflict($"{updateFundraiserDTO.status} is the same as the previous status");
                    }
                }

                if (updateFundraiserDTO.amtRequested != null)
                {
                    if (fundraiser.AmtRequested == updateFundraiserDTO.amtRequested)
                    {
                        return Conflict($"{updateFundraiserDTO.amtRequested} is the same as the previous amount requested");
                    }
                }


                if (!string.IsNullOrWhiteSpace(updateFundraiserDTO.fraName))
                {
                    bool updateSuccess = await _fundraiserActivityRepository.updateName(updateFundraiserDTO.fraName, fundraiser.Id);
                    if (!updateSuccess)
                    {
                        return StatusCode(500, "Failed to update Fundraiser Name");
                    }
                    itemsUpdated.Add(updateFundraiserDTO.fraName);
                }
                if (!string.IsNullOrWhiteSpace(updateFundraiserDTO.description))
                {
                    bool updateSuccess = await _fundraiserActivityRepository.UpdateDesc(updateFundraiserDTO.description, fundraiser.Id);
                    if (!updateSuccess)
                    {
                        return StatusCode(500, "Failed to update Fundraiser Description");
                    }
                    itemsUpdated.Add(updateFundraiserDTO.description);

                }
                if (!string.IsNullOrWhiteSpace(updateFundraiserDTO.deadline))
                {
                    bool convertToDateTimesuccess = DateTime.TryParseExact(updateFundraiserDTO.deadline,
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
                    itemsUpdated.Add(updateFundraiserDTO.deadline);

                }
                if (updateFundraiserDTO.status != null)
                {
                    bool updateSuccess = await _fundraiserActivityRepository.UpdateStatus(updateFundraiserDTO.status, fundraiser.Id);
                    if (!updateSuccess)
                    {
                        return StatusCode(500, "Failed to update Fundraiser Status");
                    }
                    itemsUpdated.Add(updateFundraiserDTO.status.ToString());

                }

                if (updateFundraiserDTO.amtRequested != null)
                {
                    bool updateSuccess = await _fundraiserActivityRepository.UpdateAmtRequested(updateFundraiserDTO.amtRequested, fundraiser.Id);

                    if (!updateSuccess)
                    {
                        return StatusCode(500, "Failed to update Fundraiser amount requested");
                    }
                    itemsUpdated.Add(updateFundraiserDTO.amtRequested.ToString());
                }
                if (updateFundraiserDTO.fraCategoryId != null)
                {
                    if(await _categoryRepository.GetById(updateFundraiserDTO.fraCategoryId) == null)
                    {
                        return BadRequest("no such fundraiser category");
                    }
                    bool updateSuccess = await _fundraiserActivityRepository.UpdateFraCate(updateFundraiserDTO.fraCategoryId, fundraiser.Id);

                    if (!updateSuccess)
                    {
                        return StatusCode(500, "Failed to update Fundraiser amount requested");
                    }
                    itemsUpdated.Add(updateFundraiserDTO.fraCategoryId.ToString());
                }

                return Ok(itemsUpdated);
            }
            return BadRequest($"Fundraiser Activity {fundraiser.Name} doesnt belong to {User.FindFirstValue(ClaimTypes.Name)}");
        }
    }
}
