using CSIT_314_Group.Data;
using CSIT_314_Group.DTO.UserAccountDTO;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.UserAccountControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchUserAccountController : ControllerBase
    {
        private readonly UserAccountRepository _userAccountRepository;
        public SearchUserAccountController(UserAccountRepository userAccountRepository)
        {
            _userAccountRepository = userAccountRepository;
        }
        [HttpPost]
        public async Task<IActionResult> Search([FromBody] SearchUserAccountDTO searchUserAccountDTO)
        {
            object? result = await _userAccountRepository.GetIdWithNameOrEmailOrPhone(searchUserAccountDTO.NameOrEmailOrPhone.ToLower());
            if(result == null)
            {
                return NotFound("User not found");
            }
            UserAccountDTO user = await _userAccountRepository.GetById(Convert.ToInt32(result));

            return Ok(user);
        }
    }
}
