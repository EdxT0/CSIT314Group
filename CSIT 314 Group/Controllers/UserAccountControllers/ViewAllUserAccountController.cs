using CSIT_314_Group.Data;
using CSIT_314_Group.DTO.UserAccountDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.UserAccountControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewAllUserAccountController : ControllerBase
    {
        private readonly UserAccountRepository _userAccountRepo;
        public ViewAllUserAccountController(UserAccountRepository userAccountRepo)
        {
            _userAccountRepo = userAccountRepo;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<List<UserAccountDTO>> ViewAllUserAccounts()
        {
            return await _userAccountRepo.ViewAllUserAccount();

            
        }
    }
}
