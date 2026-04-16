using CSIT_314_Group.Data;
using CSIT_314_Group.DTO;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.UserAccount
{
    [ApiController]
    [Route("[Controller]")]
    public class SearchUserAccountController : ControllerBase
    {
        private readonly UserAccountRepository _userAccountRepository;
        public SearchUserAccountController(UserAccountRepository userAccountRepository)
        {
            _userAccountRepository = userAccountRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string NameOrEmailOrPhone)
        {
            object? result = await _userAccountRepository.GetIdWithNameOrEmailOrPhone(NameOrEmailOrPhone.ToLower());
            if(result == null)
            {
                return NotFound("User not found");
            }
            UserAccountDTO user = await _userAccountRepository.GetById(Convert.ToInt32(result));

            return Ok(user);
        }
    }
}
