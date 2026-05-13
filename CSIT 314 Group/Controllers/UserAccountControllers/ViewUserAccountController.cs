using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CSIT_314_Group.Controllers.UserAccountControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewUserAccountController : ControllerBase
    {
        private readonly UserAccount _userAccountRepository;

        public ViewUserAccountController(UserAccount userAccountRepository)
        {
            _userAccountRepository = userAccountRepository;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ViewUserAccount([FromQuery]int userId)
        {

            UserAccount userDTO = await _userAccountRepository.GetById(userId);
            if(userDTO != null)
            {
                return Ok(userDTO);
            }
            return BadRequest("couldnt find user");
        }
    }
}

