using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.UserAccountControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchUserAccountController : ControllerBase
    {
        private readonly Data.UserAccount _userAccountRepository;
        public SearchUserAccountController(UserAccount userAccountRepository)
        {
            _userAccountRepository = userAccountRepository;
        }
        [HttpPost]
        public async Task<IActionResult> Search([FromQuery] string query)
        {


            List<UserAccount> users =  await _userAccountRepository.GetAllWithQuery(query);  
            
            return Ok(users);
        }
    }
}
