using CSIT_314_Group.Data;
using CSIT_314_Group.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CSIT_314_Group.Controllers.UserAccount
{
    [ApiController]
    [Route("[Controller]")]
    public class ViewUserAccountController : ControllerBase
    {

        private readonly UserAccountRepository _userAccountRepository;

        public ViewUserAccountController(UserAccountRepository userAccountRepository)
        {
            _userAccountRepository = userAccountRepository;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ViewUserAccount()
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if( user == null)
            {
                return Unauthorized();
            }
            
            int userId = Convert.ToInt32(user);

            UserAccountDTO userDTO = await _userAccountRepository.GetById(userId);

            return Ok(userDTO);
        }
    }
}
