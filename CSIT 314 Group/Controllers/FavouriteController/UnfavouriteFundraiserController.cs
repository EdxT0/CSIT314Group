using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CSIT_314_Group.Controllers.FavouriteController
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnfavouriteFundraiserController : ControllerBase
    {
        private readonly Favourite _favouriteRepository;


        public UnfavouriteFundraiserController(Favourite favouriteRepository)
        {
            _favouriteRepository = favouriteRepository;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UnfavouriteFundraiser([FromBody] Favourite favouriteFundraiserDTO)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier);
            if (user == null)
            {
                return NotFound("no logged in user found");
            }
            int userId = Convert.ToInt32(user.Value);
            bool favouriteResult = await _favouriteRepository.UnfavouriteFundraiser(userId, favouriteFundraiserDTO.FraId);
            if (favouriteResult)
            {
                return Ok("fundraiser unfavourited");
            }
            return BadRequest("favourite doesnt exist");
        }
    }
}
