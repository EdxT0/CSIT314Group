using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CSIT_314_Group.Controllers.FavouriteController
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchFavouriteController : ControllerBase
    {
        private readonly FavouriteRepository _favouriteRepository;

        public SearchFavouriteController(FavouriteRepository favouriteRepository)
        {
            _favouriteRepository = favouriteRepository;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SearchFavourite([FromQuery] string fraName)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier);
            if (user == null)
            {
                return BadRequest("user not logged in");
            }
            int userId = Convert.ToInt32(user.Value);
            fraName = fraName.ToLower();

            var result = await _favouriteRepository.SearchFavourites(fraName, userId);

            if (result.Count == 0)
            {
                return Ok("No favourites found");
            }
            return Ok(result);
        }
    }
}
