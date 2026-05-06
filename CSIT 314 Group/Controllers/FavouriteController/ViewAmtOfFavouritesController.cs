using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.FavouriteController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewAmtOfFavouritesController : ControllerBase
    {
        private readonly Favourite _favouriteRepository;

        public ViewAmtOfFavouritesController(Favourite favouriteRepository)
        {
            _favouriteRepository = favouriteRepository;
        }

        [HttpGet]
        public async Task<IActionResult> ViewAmtOfFavourites([FromBody] Favourite favouriteFundraiserDTO)
        {
            int? result = await _favouriteRepository.GetAmtOfFavourites(favouriteFundraiserDTO.FraId);
            if(result != null)
            {
                return Ok(result);
            }
            return BadRequest("fra not found");

        }
    }
}
