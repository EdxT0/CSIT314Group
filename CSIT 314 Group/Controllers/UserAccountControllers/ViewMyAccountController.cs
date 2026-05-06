using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CSIT_314_Group.Controllers.UserAccountControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ViewMyAccountController : ControllerBase
    {

        private readonly Data.UserAccount _userAccountRepository;

        public ViewMyAccountController(Data.UserAccount userAccountRepository)
        {
            _userAccountRepository = userAccountRepository;
        }

       // [Authorize]
        [HttpGet]
        public async Task<IActionResult> ViewMyAccount()
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if( user == null)
            {
                return Unauthorized();
            }
            
            int userId = Convert.ToInt32(user);

            UserAccount userDTO = await _userAccountRepository.GetById(userId);

            return Ok(userDTO);
        }
    }
}
