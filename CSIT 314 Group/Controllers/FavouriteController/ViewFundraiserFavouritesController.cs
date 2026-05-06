using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CSIT_314_Group.Controllers.FavouriteController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewFundraiserFavouritesController : ControllerBase
    {
        private readonly Favourite _favouriteRepository;

        public ViewFundraiserFavouritesController(Favourite favouriteRepository)
        {
            _favouriteRepository = favouriteRepository;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ViewFundraiserFavourites()
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier);
            if(user == null)
            {
                return BadRequest("no logged in user found");
            }
            int userId = Convert.ToInt32(user.Value);
            var result = await _favouriteRepository.GetFavouritesList(userId);
            if(result.Count == 0)
            {
                return NotFound("No favourites yet");
            }
            return Ok(result);

        }
    }
}
