using CSIT_314_Group.Data;
using CSIT_314_Group.DTO.FavouriteDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.FavouriteController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewAmtOfFavouritesController : ControllerBase
    {
        private readonly FavouriteRepository _favouriteRepository;

        public ViewAmtOfFavouritesController(FavouriteRepository favouriteRepository)
        {
            _favouriteRepository = favouriteRepository;
        }

        [HttpGet]
        public async Task<IActionResult> ViewAmtOfFavourites([FromBody] FavouriteFundraiserDTO favouriteFundraiserDTO)
        {
            int? result = await _favouriteRepository.GetAmtOfFavourites(favouriteFundraiserDTO.fraId);
            if(result != null)
            {
                return Ok(result);
            }
            return BadRequest("fra not found");

        }
    }
}
