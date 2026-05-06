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
