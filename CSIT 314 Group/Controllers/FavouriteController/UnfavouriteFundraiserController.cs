using CSIT_314_Group.Data;
using CSIT_314_Group.DTO.FavouriteDTO;
using CSIT_314_Group.Results;
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
        private readonly FavouriteRepository _favouriteRepository;


        public UnfavouriteFundraiserController(FavouriteRepository favouriteRepository)
        {
            _favouriteRepository = favouriteRepository;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UnfavouriteFundraiser([FromBody] FavouriteFundraiserDTO favouriteFundraiserDTO)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier);
            if (user == null)
            {
                return NotFound("no logged in user found");
            }
            int userId = Convert.ToInt32(user.Value);
            bool favouriteResult = await _favouriteRepository.UnfavouriteFundraiser(userId, favouriteFundraiserDTO.fraId);
            if (favouriteResult)
            {
                return Ok("fundraiser unfavourited");
            }
            return BadRequest("favourite doesnt exist");
        }
    }
}
