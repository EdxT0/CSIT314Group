using CSIT_314_Group.Data;
using CSIT_314_Group.DTO.UserAccountDTO;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.UserAccountControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchUserAccountController : ControllerBase
    {
        private readonly Data.UserAccount _userAccountRepository;
        public SearchUserAccountController(Data.UserAccount userAccountRepository)
        {
            _userAccountRepository = userAccountRepository;
        }
        [HttpPost]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            List<int> userIds = new List<int>();
            userIds = await _userAccountRepository.GetIdsWithNameOrEmailOrPhone(query);
            if(userIds.Count == 0 )
            {
                return NotFound("User not found");
            }
            List<UserAccountDTO> users = new List<UserAccountDTO>();
            for(int i = 0; i < userIds.Count; i++)
            {
                users.Add(await _userAccountRepository.GetById(userIds[i]));
            }           
            return Ok(users);
        }
    }
}
