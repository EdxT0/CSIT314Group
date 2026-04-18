using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.Fundraiser
{
    public class FundraiserActivityController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
