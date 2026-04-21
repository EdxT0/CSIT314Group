using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.FavouriteController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewFundraiserFavouritesController : ControllerBase
    {
        private readonly FavouriteRepository _favouriteRepository;

        public ViewFundraiserFavouritesController(FavouriteRepository favouriteRepository)
        {
            _favouriteRepository = favouriteRepository;
        }

    }
}
