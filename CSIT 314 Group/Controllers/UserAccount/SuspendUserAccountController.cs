using CSIT_314_Group.Data;
using CSIT_314_Group.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.UserAccount
{
    [ApiController]
    [Route("[Controller]")]
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
            var userIdResult = await _userAccountRepository.GetIdWithNameOrEmailOrPhone(suspendUserDTO.email.ToLower());
            if(userIdResult == null)
            {
                return NotFound("User dont exist");
            }
            int userId = Convert.ToInt32(userIdResult);
            var isSuspended = await _userAccountRepository.GetSuspendStatusWithId(userId);
            if (Convert.ToBoolean(isSuspended) == true && suspendUserDTO.suspendUser == true)
            {
                return Conflict($"User already suspended");
            }else if (Convert.ToBoolean(isSuspended) == false && suspendUserDTO.suspendUser == false)
            {
                return Conflict($"User already unsuspended");
            }
            var boolResult = await _userAccountRepository.SuspendUserWithId(userId, suspendUserDTO.suspendUser);
            if (Convert.ToBoolean(boolResult) && suspendUserDTO.suspendUser == true)
            {
                return Ok("User suspended");
            }else if (Convert.ToBoolean(boolResult) && suspendUserDTO.suspendUser == false)
            {
                return Ok("User unsuspended");
            }
            return BadRequest();

        }
    }
}
