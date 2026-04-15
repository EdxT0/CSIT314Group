using CSIT_314_Group.Data;
using CSIT_314_Group.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        public async Task<IActionResult> ViewUserAccount([FromQuery] string nameOrEmail)
        {
            object? result = await _userAccountRepository.GetIdWithNameOrEmail(nameOrEmail.ToLower());

            if(result== null)
            {
                return NotFound("User Not Found");
            }
            int userId = Convert.ToInt32(result);

            UserAccountDTO user = await _userAccountRepository.GetById(userId);

            return Ok(user);
        }
    }
}
