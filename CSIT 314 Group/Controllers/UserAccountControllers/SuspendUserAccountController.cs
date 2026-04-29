using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CSIT_314_Group.DTO.UserAccountDTO;


namespace CSIT_314_Group.Controllers.UserAccountControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuspendUserAccountController : ControllerBase
    {
        private readonly UserAccountRepository _userAccountRepository;
        public SuspendUserAccountController(UserAccountRepository userAccountRepository)
        {
            _userAccountRepository = userAccountRepository;
        }

        [Authorize(Roles = "admin")]
        [HttpPut]
        public async Task<IActionResult> SuspendUserAccount([FromBody] SuspendUserDTO suspendUserDTO)
        {
            var user = await _userAccountRepository.GetById(suspendUserDTO.userId);
            if( user == null)
            {
                return BadRequest("User Not Found");
            }
            var isSuspended = await _userAccountRepository.GetSuspendStatusWithId(suspendUserDTO.userId);
            if (isSuspended == true && suspendUserDTO.SuspendUser == true)
            {
                return Conflict($"User already suspended");
            }else if (isSuspended == true == false && suspendUserDTO.SuspendUser == false)
            {
                return Conflict($"User already unsuspended");
            }
            var boolResult = await _userAccountRepository.SuspendUserWithId(suspendUserDTO.userId, suspendUserDTO.SuspendUser);
            if (boolResult && suspendUserDTO.SuspendUser == true)
            {
                return Ok("User suspended");
            }else if (boolResult && suspendUserDTO.SuspendUser == false)
            {
                return Ok("User unsuspended");
            }
            return BadRequest();

        }
    }
}
