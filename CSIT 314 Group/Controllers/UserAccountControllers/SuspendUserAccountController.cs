using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CSIT_314_Group.Controllers.UserAccountControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuspendUserAccountController : ControllerBase
    {
        private readonly UserAccount _userAccountRepository;
        public SuspendUserAccountController(Data.UserAccount userAccountRepository)
        {
            _userAccountRepository = userAccountRepository;
        }

        [Authorize(Roles = "admin")]
        [HttpPut]
        public async Task<IActionResult> SuspendUserAccount([FromQuery] int userId , [FromQuery] bool suspendUser)
        {
            var user = await _userAccountRepository.GetById(userId);
            if( user == null)
            {
                return BadRequest("User Not Found");
            }
            var isSuspended = await _userAccountRepository.GetSuspendStatusWithId(userId);
            if (isSuspended == true && suspendUser == true)
            {
                return Conflict($"User already suspended");
            }else if (isSuspended == true == false && suspendUser == false)
            {
                return Conflict($"User already unsuspended");
            }
            var boolResult = await _userAccountRepository.SuspendUserWithId(userId, suspendUser);
            if (boolResult && suspendUser == true)
            {
                return Ok("User suspended");
            }else if (boolResult && suspendUser == false)
            {
                return Ok("User unsuspended");
            }
            return BadRequest();

        }
    }
}
