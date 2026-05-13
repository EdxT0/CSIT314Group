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
        public async Task<IActionResult> FavouriteFundraiser([FromBody] Favourite favouriteFundraiser)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier);
            if(user == null)
            {
                return NotFound("no logged in user found");
            }
            var fundraiserExist = await _fundraiserActivity.GetById(favouriteFundraiser.FraId);
            if(fundraiserExist != null)
            {
                int userId = Convert.ToInt32(user.Value);
                var result = await _favouriteRepository.FavouriteFundraiser(userId, favouriteFundraiser.FraId);
                
               return Ok(result);
                
                
            }
            return BadRequest("fundraiser doesnt exist");
        }
    }
}
