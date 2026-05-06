using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace CSIT_314_Group.Controllers.FavouriteController
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavouriteFundraiserController : ControllerBase
    {
        private readonly Favourite _favouriteRepository;
        private readonly Data.FundraiserActivity _fundraiserActivity;

        public FavouriteFundraiserController(Favourite favouriteRepository, Data.FundraiserActivity fundraiserActivity)
        {
            _favouriteRepository = favouriteRepository;
            _fundraiserActivity = fundraiserActivity;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> FavouriteFundraiser([FromBody] Favourite favouriteFundraiserDTO)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier);
            if(user == null)
            {
                return NotFound("no logged in user found");
            }
            var fundraiserExist = await _fundraiserActivity.GetById(favouriteFundraiserDTO.FraId);
            if(fundraiserExist != null)
            {
                int userId = Convert.ToInt32(user.Value);
                var result = await _favouriteRepository.FavouriteFundraiser(userId, favouriteFundraiserDTO.FraId);
                if (result.success)
                {
                    return Ok(result.message);
                }
                else
                {
                    return BadRequest(result.message);
                }
            }
            return BadRequest("fundraiser doesnt exist");
        }
    }
}
