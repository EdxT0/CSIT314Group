using CSIT_314_Group.Data;
using CSIT_314_Group.Results;
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
                FavouriteFundraiserResultEnum favouriteFundraiserResultEnum = await _favouriteRepository.FavouriteFundraiser(userId, favouriteFundraiserDTO.FraId);
                return favouriteFundraiserResultEnum switch
                {
                    FavouriteFundraiserResultEnum.success => Ok("Fundraiser favourited"),
                    FavouriteFundraiserResultEnum.duplicate => Conflict("Fundraiser is already favourited"),
                    FavouriteFundraiserResultEnum.failed => BadRequest("failed to favourite fundraiser")
                };
            }
            return BadRequest("fundraiser doesnt exist");
        }
    }
}
